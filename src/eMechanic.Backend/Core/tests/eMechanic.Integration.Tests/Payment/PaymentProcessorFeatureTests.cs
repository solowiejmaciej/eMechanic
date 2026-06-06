namespace eMechanic.Integration.Tests.Payment;

using System.Net;
using System.Text;
using API.Constans;
using API.Features.Payment;
using Domain.Payment;
using Domain.Payment.Enums;
using Domain.Repair.Enums;
using FluentAssertions;
using Helpers;
using Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repair;

[Collection("Sequential")]
public class PaymentProcessorFeatureTests : IClassFixture<PaymentIntegrationTestWebAppFactory>
{
    private readonly PaymentIntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";
    private const string MockProviderSessionId = "cs_test_mock_session_001";

    public PaymentProcessorFeatureTests(PaymentIntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Processor_Should_SetRepairStatusToPaid_WhenPaymentConfirmedByStripe()
    {
        var scenario = await RepairScenarioHelper.CreateScheduledRepairAsync(_client, _factory);

        using (var setupScope = _factory.Services.CreateScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var repair = await db.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
            repair.StartRepair();
            repair.CompleteRepair(Domain.Shared.ValueObjects.Money.Create(2000m, "PLN").Value!);
            repair.ClearDomainEvents();

            var paymentOrder = PaymentOrder.Create(
                scenario.RepairId,
                EPayableType.Repair,
                Domain.Shared.ValueObjects.Money.Create(2000m, "PLN").Value!,
                Guid.NewGuid());
            paymentOrder.StartCheckout(MockProviderSessionId, "https://checkout.stripe.com/pay/cs_test_mock");

            await db.PaymentOrders.AddAsync(paymentOrder);
            await db.SaveChangesAsync();
        }

        _factory.MockProcessor.Configure(
            scenario.RepairId, EPayableType.Repair, MockProviderSessionId);

        var processorUrl = $"{BASE_API_URL}{PaymentPrefix.WEBHOOK}";

        using var request = new HttpRequestMessage(HttpMethod.Post, processorUrl)
        {
            Content = new StringContent("{\"type\":\"checkout.session.completed\"}", Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("Stripe-Signature", "t=123456,v1=test-signature");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var assertScope = _factory.Services.CreateScope();
        var assertDb = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var paidRepair = await assertDb.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
        paidRepair.Status.Should().Be(ERepairStatus.Paid);

        var paidOrder = await assertDb.PaymentOrders.SingleAsync(o => o.ProviderSessionId == MockProviderSessionId);
        paidOrder.Status.Should().Be(EPaymentOrderStatus.Paid);
    }

    [Fact]
    public async Task Processor_Should_BeIdempotent_WhenProcessedTwice()
    {
        var scenario = await RepairScenarioHelper.CreateScheduledRepairAsync(_client, _factory);
        const string providerSessionId = "cs_test_idempotent_001";

        using (var setupScope = _factory.Services.CreateScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var repair = await db.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
            repair.StartRepair();
            repair.CompleteRepair(Domain.Shared.ValueObjects.Money.Create(2000m, "PLN").Value!);
            repair.Pay();
            repair.ClearDomainEvents();

            var paymentOrder = PaymentOrder.Create(
                scenario.RepairId,
                EPayableType.Repair,
                Domain.Shared.ValueObjects.Money.Create(2000m, "PLN").Value!,
                Guid.NewGuid());
            paymentOrder.StartCheckout(providerSessionId, "https://checkout.stripe.com/pay/cs_test_idempotent");
            paymentOrder.Complete();

            await db.PaymentOrders.AddAsync(paymentOrder);
            await db.SaveChangesAsync();
        }

        _factory.MockProcessor.Configure(scenario.RepairId, EPayableType.Repair, providerSessionId);

        var processorUrl = $"{BASE_API_URL}{PaymentPrefix.WEBHOOK}";
        using var request = new HttpRequestMessage(HttpMethod.Post, processorUrl)
        {
            Content = new StringContent("{\"type\":\"checkout.session.completed\"}", Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("Stripe-Signature", "t=123456,v1=test-signature");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Processor_Should_Return400_WhenSignatureIsInvalid()
    {
        _factory.MockProcessor.PayableItemToReturn = null;

        var processorUrl = $"{BASE_API_URL}{PaymentPrefix.WEBHOOK}";
        var response = await _client.PostAsync(
            processorUrl,
            new StringContent("{}", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
