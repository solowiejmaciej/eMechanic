# AGENTS.md

## Purpose
- This repo is a .NET 9, multi-project backend for eMechanic with one core API, one notification API, and one Azure Functions outbox publisher.
- No prior agent-specific instructions were found (only a minimal `README.md`), so this guide is derived from code conventions and tests.

## Architecture at a glance
- Solution layout: `Core/src` (API/Application/Domain/Infrastructure), `Common/src` (shared abstractions + events), `Services/src` (NotificationService + OutboxPublisher), `eMechanic.AppHost` (Aspire orchestration).
- Service orchestration is centralized in `eMechanic.AppHost/AppHost.cs` (Postgres, Redis, Azure Service Bus, Storage, projects wiring, env vars).
- HTTP flow: Feature endpoint (`Core/src/eMechanic.API/Features/**`) -> MediatR command/query -> handler in Application -> repositories/services in Infrastructure.
- Event flow: `AppDbContext.SaveChangesAsync` writes outbox rows + publishes in-process domain events (`Core/src/eMechanic.Infrastructure/DAL/AppDbContext.cs`), then `Services/src/eMechanic.OutboxPublisher/OutboxPublisherFunction.cs` publishes to Service Bus.
- Consumer flow: Notification service registers consumers from assembly (`AddEventConsuming`) and consumes events like `UserCreatedConsumer`.

## Hard conventions (enforced by tests)
- Check `Core/tests/eMechanic.Architecture.Tests/*` before refactoring structure.
- API features must implement `IFeature`, be `sealed`, and stay under `eMechanic.API.Features...`.
- Handlers must implement custom CQRS interfaces (`IResultCommandHandler` / `IResultQueryHandler`) and live in `eMechanic.Application...`.
- Commands/queries naming is strict: `*Command`, `*Query`, handlers `*Handler`.
- Feature classes should not depend directly on Infrastructure/Domain (use MediatR).
- **DDD Encapsulation:** Domain Aggregates and Entities must have private/protected setters and parameters-less constructors. Business state modifications must go through explicit domain methods (e.g., `Vehicle.UpdateMileage`) which actively protect business invariants.
- **Value Objects:** Always encapsulate domain primitives using immutable value objects (`readonly record struct` or custom types with structural equality like `Vin`, `Mileage`, `Money`, `LicensePlate`) instead of using raw strings or decimals inside entities.
- **Domain Events:** State mutations within Aggregates must track and register domain events using `AddDomainEvent(IDomainEvent)`.

## Endpoint and result pattern
- Endpoint discovery is reflection-based via `MapFeatures()` (`Common/src/eMechanic.Common/Web/FeatureExtensions.cs`); new `IFeature` classes auto-map.
- Route root is versioned: `/api/v1` (see `Core/src/eMechanic.API/Constans/WebApiConstans.cs`).
- Feature pattern example: `CreateUserFeature` maps HTTP request -> command -> `result.ToStatusCode(...)` with `ErrorMapper`.
- **CQRS Records:** All MediatR Commands and Queries MUST be defined as `record` types to ensure immutability and precise structural/value-based equality for behavioral pipelines.
- Use `Result<T, Error>` return style (`Common/src/eMechanic.Common/Result/*`) for command/query handlers. Do not throw business exceptions; return failing results. Validation failures via FluentValidation are handled automatically via MediatR Pipeline behaviors.
- **Thin API Layer:** Feature classes must remain extremely thin, handling only route mappings and immediate MediatR dispatching. No business logic, mapping, or transformations should live here.

## Infra and config behavior to remember
- API auto-applies EF Core migrations on startup (`ApplyMigrations` call in `Core/src/eMechanic.API/DependencyInjection.cs`).
- **Double DB Contexts:** Be aware that identity storage and refresh tokens use `IdentityAppDbContext`, while core business operations (Vehicles, Workshops, Repairs) use `AppDbContext`.
- Cache behavior: Redis when `emechanic-cache` connection string exists, otherwise in-memory cache fallback (`AddDistributedMemoryCache`).
- Query/command validation and caching are MediatR open behaviors registered in `Core/src/eMechanic.Application/DependencyInjection.cs`.
- **Deterministic Cache Keys:** Caching keys are built deterministically inside `CachingBehavior` by serializing the request record values into minified JSON and hashing it with SHA256 (or via an explicit `KeyFactory`). *Never rely on raw memory-reference `.GetHashCode()` calls within generic pipeline keys.*
- Caching rules are attribute-driven (`[Cache(...)]`) and registered by assembly scan at startup.
- LLM facade returns a stub message in Development; real model calls require provider config (`LLMProviders:*`).
- `AddAzureAppConfiguration` only activates in Production and requires `AzureAppConfig` env var.

## Build/test/run workflows (from repo root)
```bash
dotnet restore eMechanic.sln
dotnet build eMechanic.sln -c Debug
dotnet run --project eMechanic.AppHost/eMechanic.AppHost.csproj
dotnet test eMechanic.sln
dotnet test Core/tests/eMechanic.Architecture.Tests/eMechanic.Architecture.Tests.csproj
dotnet test Core/tests/eMechanic.Integration.Tests/eMechanic.Integration.Tests.csproj
```
Domain Tests: Must be 100% pure unit tests focusing on state mutations and domain event publishing inside entities, written without any database mocks.

Application Tests: Target CQRS Handlers. All infrastructure dependencies (repositories, context facades, external services) MUST be mocked using NSubstitute.

Integration tests use Testcontainers + PostgreSQL (IntegrationTestWebAppFactory), so Docker must be running. Every new API endpoint requires an integration feature test using FluentAssertions syntax exclusively (e.g., result.IsSuccess.Should().BeTrue()).

Health endpoints from service defaults: /health and `/alive$; in Development root redirects to Swagger.

Event bus specifics
Event transport uses MassTransit Azure Service Bus (Common/src/eMechanic.Events/DependencyInjection.cs).

Consumer queue naming convention is ${entryAssemblyName}.${messageTypeName}; changing assembly names changes queue names.

Outbox publisher processes up to 20 rows per run (FOR UPDATE SKIP LOCKED, timer every minute).
