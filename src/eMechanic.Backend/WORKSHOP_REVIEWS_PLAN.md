# Workshop Reviews Plan (Low Priority)

This item is intentionally tracked as low priority.

## Goal
Add a workshop reviews module to close the gap represented by the empty `Core/src/eMechanic.Domain/Workshop/Reviews/` area.

## Proposed Scope
1. Domain
   - Add `Review` aggregate/entity for `WorkshopId`, `UserId`, rating, optional comment, timestamps.
   - Enforce one review per user per workshop (or support updates to existing review).
   - Raise domain events on create/update/delete.
2. Application
   - Add CQRS commands/queries for create/update/get reviews.
   - Add paginated and searchable workshop review listing.
   - Add average rating projection/query.
3. API
   - Add feature endpoints under workshop routes.
   - Protect write operations with user authorization.
4. Infrastructure
   - EF configuration, migrations, indexes.
   - Optional outbox integration events for notifications.
5. Tests
   - Domain tests for invariants.
   - Application tests for handlers.
   - Integration tests for new endpoints.

## Acceptance Criteria
- Users can add/update review for workshop.
- Workshop details can return review stats.
- Pagination and search are supported.
- Full test coverage in domain/application/integration layers.

