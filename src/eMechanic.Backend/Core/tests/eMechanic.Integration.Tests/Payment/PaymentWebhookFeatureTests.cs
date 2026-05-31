namespace eMechanic.Integration.Tests.Payment;

using System.Net;
using System.Text;
using API.Constans;
using API.Features.Payment;
using Application.Payments.Common;
using Domain.Repair.Enums;
using FluentAssertions;
using Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repair;

[Collection("Sequential")]
public class PaymentWebhookFeatureTests : IClassFixture<PaymentIntegrationTestWebAppFactory>
{
    private readonly PaymentIntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public PaymentWebhookFeatureTests(PaymentIntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Webhook_Should_SetRepairStatusToPaid_WhenPaymentConfirmedByStripe()
    {
        // Arrange - create a repair and advance it to Completed state
        var scenario = await RepairScenarioHelper.CreateScheduledRepairAsync(_client, _factory);

        using (var setupScope = _factory.Services.CreateScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var repair = await db.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
            repair.StartRepair();
            repair.CompleteRepair(Domain.Shared.ValueObjects.Money.Create(2000m, "PLN").Value!);

            repair.ClearDomainEvents();
            await db.SaveChangesAsync();
        }

        _factory.MockWebhookProcessor.Configure(scenario.RepairId, EPayableType.Repair, 2000m, "PLN");

        var webhookUrl = $"{BASE_API_URL}{PaymentPrefix.WEBHOOK}";

        // Act - send webhook payload with required signature header
        using var request = new HttpRequestMessage(HttpMethod.Post, webhookUrl)
        {
            Content = new StringContent("{\"type\":\"checkout.session.completed\"}", Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("Stripe-Signature", "t=123456,v1=test-signature");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var assertScope = _factory.Services.CreateScope();
        var assertDb = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var paidRepair = await assertDb.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
        paidRepair.Status.Should().Be(ERepairStatus.Paid);
    }

    [Fact]
    public async Task Webhook_Should_Return400_WhenSignatureIsInvalid()
    {
        // Arrange - processor is not configured, so it returns an error
        _factory.MockWebhookProcessor.PayableItemToReturn = null;

        var webhookUrl = $"{BASE_API_URL}{PaymentPrefix.WEBHOOK}";

        // Act
        var response = await _client.PostAsync(
            webhookUrl,
            new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert - validator requires SignatureHeader to be non-empty
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
