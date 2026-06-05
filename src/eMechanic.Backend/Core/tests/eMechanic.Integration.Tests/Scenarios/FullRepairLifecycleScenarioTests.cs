namespace eMechanic.Integration.Tests.Scenarios;

using System.Net;
using System.Net.Http.Json;
using System.Text;
using API.Constans;
using API.Features.Payment;
using API.Features.Repair;
using API.Features.Repair.Complete;
using API.Features.RepairRequest;
using API.Features.RepairRequest.ProvideEstimation;
using API.Features.Vehicle.Vehicle.Create.Request;
using API.Features.Workshop;
using API.Features.Workshop.Reviews.Request;
using Application.Payments.Common;
using Application.Repair.Features.Get.ById;
using Application.RepairRequest.Features.Create;
using Application.Workshop.Reviews.Features.Get;
using Common.Result;
using Domain.Payment.Enums;
using Domain.Repair.Enums;
using FluentAssertions;
using Helpers;
using Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payment;

[Collection("Sequential")]
public sealed class FullRepairLifecycleScenarioTests : IClassFixture<PaymentIntegrationTestWebAppFactory>
{
    private readonly PaymentIntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public FullRepairLifecycleScenarioTests(PaymentIntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact]
    public async Task FullRepairLifecycle_ShouldCompleteAllSteps_EndToEnd()
    {
        var userAuth     = await _authHelper.CreateAndLoginUserAsync();
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();

        userAuth.Token.Should().NotBeNullOrWhiteSpace("user musi mieć JWT");
        workshopAuth.Token.Should().NotBeNullOrWhiteSpace("warsztat musi mieć JWT");

        var userId     = userAuth.DomainId;
        var workshopId = workshopAuth.DomainId;

        _client.SetBearerToken(userAuth.Token);

        var createVehicleRequest = new CreateVehicleRequest(
            Vin:             $"JH4{Guid.NewGuid().ToString("N")[..14]}",
            Manufacturer:    "Toyota",
            Model:           "Corolla",
            ProductionYear:  "2019",
            EngineCapacity:  1.8m,
            MileageValue:    62000,
            MileageUnit:     Domain.Vehicle.Vehicle.Enums.EMileageUnit.Kilometers,
            LicensePlate:    "KR12999",
            HorsePower:      140,
            FuelType:        Domain.Vehicle.Vehicle.Enums.EFuelType.Gasoline,
            BodyType:        Domain.Vehicle.Vehicle.Enums.EBodyType.Sedan,
            VehicleType:     Domain.Vehicle.Vehicle.Enums.EVehicleType.Passenger);

        var createVehicleResponse = await _client.PostAsJsonAsync(
            $"{BASE}{API.Features.Vehicle.VehiclePrefix.CREATE}", createVehicleRequest);
        createVehicleResponse.StatusCode.Should().Be(HttpStatusCode.Created, "pojazd powinien zostać utworzony");

        var vehicleContent = await createVehicleResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vehicleContent!["vehicleId"];
        vehicleId.Should().NotBeEmpty("vehicleId musi być zwrócony");


        var createRepairRequestCommand = new CreateRepairRequestCommand(
            vehicleId,
            workshopId,
            "Silnik stuka przy zimnym starcie, temperatura oleju skacze – proszę o diagnostykę.");

        var createRepairRequestResponse = await _client.PostAsJsonAsync(
            $"{BASE}{RepairRequestPrefix.CREATE}", createRepairRequestCommand);
        createRepairRequestResponse.StatusCode.Should().Be(HttpStatusCode.Created, "zlecenie naprawy powinno zostać utworzone");

        var repairRequestContent = await createRepairRequestResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var repairRequestId = repairRequestContent!["repairRequestId"];
        repairRequestId.Should().NotBeEmpty("repairRequestId musi być zwrócony");

        _client.SetBearerToken(workshopAuth.Token);

        var provideEstimationRequest = new ProvideRepairEstimationRequest("Uszkodzony łańcuch rozrządu i uszczelka pod głowicą.", 2400m, "PLN");
        var provideEstimationUrl     = $"{BASE}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";

        var provideEstimationResponse = await _client.PutAsJsonAsync(provideEstimationUrl, provideEstimationRequest);
        provideEstimationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "wycena powinna zostać zapisana");

        _client.SetBearerToken(userAuth.Token);

        var acceptUrl      = $"{BASE}{RepairRequestPrefix.ACCEPT_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";
        var acceptResponse = await _client.PutAsync(acceptUrl, null);
        acceptResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "akceptacja wyceny powinna się udać");

        Guid repairId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var repair = await db.Repairs.SingleAsync(r => r.RepairRequestId == repairRequestId);
            repair.Status.Should().Be(ERepairStatus.Scheduled, "po akceptacji naprawa powinna być w statusie Scheduled");
            repairId = repair.Id;
        }

        _client.SetBearerToken(workshopAuth.Token);

        var startUrl      = $"{BASE}{RepairPrefix.START.Replace("{id}", repairId.ToString())}";
        var startResponse = await _client.PutAsync(startUrl, null);
        startResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "warsztat powinien móc rozpocząć naprawę");

        using (var scope = _factory.Services.CreateScope())
        {
            var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var repair = await db.Repairs.SingleAsync(r => r.Id == repairId);
            repair.Status.Should().Be(ERepairStatus.InProgress, "po starcie naprawa powinna być InProgress");
        }


        var completeRequest  = new CompleteRepairRequest(2550m, "PLN");
        var completeUrl      = $"{BASE}{RepairPrefix.COMPLETE.Replace("{id}", repairId.ToString())}";
        var completeResponse = await _client.PutAsJsonAsync(completeUrl, completeRequest);
        completeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "warsztat powinien móc sfinalizować naprawę");

        using (var scope = _factory.Services.CreateScope())
        {
            var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var repair = await db.Repairs.SingleAsync(r => r.Id == repairId);
            repair.Status.Should().Be(ERepairStatus.Completed, "po zakończeniu naprawa powinna być Completed");
            repair.FinalCost.Should().NotBeNull();
            repair.FinalCost!.Amount.Should().Be(2550m);
            repair.FinalCost.Currency.Should().Be("PLN");
        }

        _client.SetBearerToken(userAuth.Token);

        const string mockSessionId = "cs_test_lifecycle_001";
        _factory.MockProcessor.Configure(repairId, EPayableType.Repair, mockSessionId, 2550m, "PLN");

        var initializePaymentRequest = new
        {
            referenceId = repairId,
            type        = (int)EPayableType.Repair,
            successUrl  = "https://emechanic.test/success",
            cancelUrl   = "https://emechanic.test/cancel"
        };

        var initPaymentUrl      = $"{BASE}{PaymentPrefix.INITIALIZE}";
        var initPaymentResponse = await _client.PostAsJsonAsync(initPaymentUrl, initializePaymentRequest);
        initPaymentResponse.StatusCode.Should().Be(HttpStatusCode.OK, "inicjalizacja płatności powinna się udać");

        var paymentSession = await initPaymentResponse.Content.ReadFromJsonAsync<PaymentSessionDto>();
        paymentSession.Should().NotBeNull();
        paymentSession!.SessionId.Should().Be(mockSessionId);
        paymentSession.CheckoutUrl.Should().NotBeNullOrWhiteSpace();

        _client.ClearBearerToken();

        var webhookUrl = $"{BASE}{PaymentPrefix.WEBHOOK}";
        using var webhookRequest = new HttpRequestMessage(HttpMethod.Post, webhookUrl)
        {
            Content = new StringContent(
                """{"type":"checkout.session.completed"}""",
                Encoding.UTF8,
                "application/json")
        };
        webhookRequest.Headers.Add("Stripe-Signature", "t=123456,v1=test-lifecycle-signature");

        var webhookResponse = await _client.SendAsync(webhookRequest);
        webhookResponse.StatusCode.Should().Be(HttpStatusCode.OK, "webhook powinien zostać przetworzony");

        using (var scope = _factory.Services.CreateScope())
        {
            var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var repair = await db.Repairs.SingleAsync(r => r.Id == repairId);
            repair.Status.Should().Be(ERepairStatus.Paid, "po płatności naprawa powinna być w statusie Paid");
        }

        _client.SetBearerToken(userAuth.Token);

        var upsertReviewUrl      = $"{BASE}{WorkshopPrefix.UPSERT_WORKSHOP_REVIEW_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}";
        var upsertReviewResponse = await _client.PutAsJsonAsync(upsertReviewUrl, new UpsertWorkshopReviewRequest(5, "Profesjonalnie, szybko i zgodnie z wyceną – polecam!"));

        upsertReviewResponse.StatusCode.Should().Be(HttpStatusCode.OK, "opinia powinna zostać zapisana");
        var reviewId = await upsertReviewResponse.Content.ReadFromJsonAsync<Guid>();
        reviewId.Should().NotBeEmpty("reviewId musi być zwrócony");

        _client.ClearBearerToken();

        var getReviewsUrl      = $"{BASE}{WorkshopPrefix.GET_WORKSHOP_REVIEWS_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}?pageNumber=1&pageSize=10";
        var getReviewsResponse = await _client.GetAsync(getReviewsUrl);
        getReviewsResponse.StatusCode.Should().Be(HttpStatusCode.OK, "pobieranie opinii powinno się udać");

        var reviewsPage = await getReviewsResponse.Content
            .ReadFromJsonAsync<PaginationResult<WorkshopReviewResponse>>();
        reviewsPage.Should().NotBeNull();
        reviewsPage!.Items.Should().ContainSingle("powinniśmy mieć dokładnie jedną opinię");

        var review = reviewsPage.Items.First();
        review.Rating.Should().Be(5);
        review.Comment.Should().Be("Profesjonalnie, szybko i zgodnie z wyceną – polecam!");
        review.UserId.Should().Be(userId);
        review.WorkshopId.Should().Be(workshopId);

        // Statystyki
        var statsUrl      = $"{BASE}{WorkshopPrefix.GET_WORKSHOP_REVIEW_STATS_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}";
        var statsResponse = await _client.GetAsync(statsUrl);
        statsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var stats = await statsResponse.Content.ReadFromJsonAsync<WorkshopReviewStatsResponse>();
        stats.Should().NotBeNull();
        stats!.WorkshopId.Should().Be(workshopId);
        stats.TotalReviews.Should().BeGreaterThanOrEqualTo(1);
        stats.AverageRating.Should().BeGreaterThan(0);


        _client.SetBearerToken(userAuth.Token);

        var getRepairUrl      = $"{BASE}{RepairPrefix.GET_BY_ID_FOR_USER.Replace("{id}", repairId.ToString())}";
        var getRepairResponse = await _client.GetAsync(getRepairUrl);
        getRepairResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var repairDetails = await getRepairResponse.Content.ReadFromJsonAsync<RepairResponse>();
        repairDetails.Should().NotBeNull();
        repairDetails!.Id.Should().Be(repairId);
        repairDetails.Status.Should().Be(ERepairStatus.Paid);
        repairDetails.FinalCost.Should().NotBeNull();
        repairDetails.FinalCost!.Amount.Should().Be(2550m);
    }
}


