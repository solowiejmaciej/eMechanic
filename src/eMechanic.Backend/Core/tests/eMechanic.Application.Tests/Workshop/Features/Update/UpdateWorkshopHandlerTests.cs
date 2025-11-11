namespace eMechanic.Application.Tests.Workshop.Features.Update
{
    using eMechanic.Application.Abstractions.Identity.Contexts;
    using eMechanic.Application.Workshop.Features.Update;
    using eMechanic.Application.Workshop.Services;
    using eMechanic.Common.Result;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    public class UpdateWorkshopHandlerTests
    {
        private readonly IWorkshopContext _workshopContext;
        private readonly IWorkshopService _workshopService;
        private readonly UpdateWorkshopHandler _handler;

        private readonly Guid _currentWorkshopId = Guid.NewGuid();

        public UpdateWorkshopHandlerTests()
        {
            _workshopContext = Substitute.For<IWorkshopContext>();
            _workshopService = Substitute.For<IWorkshopService>();
            _handler = new UpdateWorkshopHandler(_workshopContext, _workshopService);

            _workshopContext.GetWorkshopId().Returns(_currentWorkshopId);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenContextAndServiceSucceed()
        {
            // Arrange
            var command = new UpdateWorkshopCommand(
                "login@w.pl", "k@w.pl", "Nazwa", "Display", "123",
                "Adres", "Miasto", "Kod", "Kraj");

            _workshopService.UpdateWorkshopWithIdentityAsync(
                _currentWorkshopId, command.Email, command.ContactEmail, command.Name,
                command.DisplayName, command.PhoneNumber, command.Address, command.City,
                command.PostalCode, command.Country, Arg.Any<CancellationToken>())
            .Returns(Result.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.HasError().Should().BeFalse();
            await _workshopService.Received(1).UpdateWorkshopWithIdentityAsync(
                _currentWorkshopId, command.Email, command.ContactEmail, command.Name,
                command.DisplayName, command.PhoneNumber, command.Address, command.City,
                command.PostalCode, command.Country, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ReturnError_WhenServiceReturnsError()
        {
            // Arrange
            var command = new UpdateWorkshopCommand(
                "taken@w.pl", "k@w.pl", "Nazwa", "Display", "123",
                "Adres", "Miasto", "Kod", "Kraj");
            var error = new Error(EErrorCode.ValidationError, "Email taken");

            _workshopService.UpdateWorkshopWithIdentityAsync(
                Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(error);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.HasError().Should().BeTrue();
            result.Error.Should().Be(error);
        }

        [Fact]
        public async Task Handle_Should_ReturnUnauthorizedError_WhenContextThrows()
        {
            // Arrange
            var command = new UpdateWorkshopCommand(
                "login@w.pl", "k@w.pl", "Nazwa", "Display", "123",
                "Adres", "Miasto", "Kod", "Kraj");
            var authException = new UnauthorizedAccessException("Workshop not authenticated.");

            _workshopContext.GetWorkshopId().Throws(authException);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
