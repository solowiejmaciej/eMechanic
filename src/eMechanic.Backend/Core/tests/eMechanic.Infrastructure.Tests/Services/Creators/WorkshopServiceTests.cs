namespace eMechanic.Infrastructure.Tests.Services.Creators;

using Application.Abstractions.Identity;
using Application.Identity;
using Application.Workshop.Features.Update;
using Application.Workshop.Repositories;
using Common.Result;
using Common.Result.Fields;
using Domain.Workshop;
using eMechanic.Infrastructure.Identity;
using eMechanic.Infrastructure.Services.Creators;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class WorkshopServiceTests
{
    private readonly UserManager<Identity> _userManager;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<WorkshopService> _logger;
    private readonly WorkshopService _workshopService;

    private const string TEST_EMAIL = "test@workshop.com";
    private const string TEST_PASSWORD = "Password123!";
    private const string TEST_CONTACT_EMAIL = "contact@workshop.com";
    private const string TEST_NAME = "Test Workshop";
    private const string TEST_DISPLAY_NAME = "TestW";
    private const string TEST_PHONE = "123456789";
    private const string TEST_ADDRESS = "Test Address 1";
    private const string TEST_CITY = "Test City";
    private const string TEST_POSTAL_CODE = "12-345";
    private const string TEST_COUNTRY = "Testland";

    private readonly Guid _domainWorkshopId = Guid.NewGuid();
    private readonly Guid _identityId = Guid.NewGuid();
    private readonly Workshop _fakeWorkshop;
    private readonly Identity _fakeIdentity;

    public WorkshopServiceTests()
    {
        var userStoreMock = Substitute.For<IUserStore<Identity>>();
        _userManager = Substitute.For<UserManager<Identity>>(userStoreMock, null, null, null, null, null, null, null, null);

        _workshopRepository = Substitute.For<IWorkshopRepository>();
        _transactionalExecutor = Substitute.For<ITransactionalExecutor>();
        _logger = Substitute.For<ILogger<WorkshopService>>();

        _workshopService = new WorkshopService( // Zmiana
            _userManager,
            _workshopRepository,
            _transactionalExecutor,
            _logger);

        _transactionalExecutor
            .ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>())
            .Returns(async call =>
            {
                var operation = call.Arg<Func<Task>>();
                await operation();
            });

        _fakeWorkshop = Workshop.Create(
            TEST_CONTACT_EMAIL, TEST_EMAIL, TEST_DISPLAY_NAME, TEST_NAME,
            TEST_PHONE, TEST_ADDRESS, TEST_CITY, TEST_POSTAL_CODE, TEST_COUNTRY, _identityId);

        _fakeIdentity = Identity.Create(TEST_EMAIL, EIdentityType.Workshop);
        _fakeIdentity.Id = _identityId;
    }

    [Fact]
    public async Task CreateWorkshopWithIdentityAsync_Should_ReturnEmailExistsError_WhenIdentityAlreadyExists()
    {
        // Arrange
        var existingIdentity = Identity.Create(TEST_EMAIL, EIdentityType.User);
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(existingIdentity);

        // Act
        var result = await _workshopService.CreateWorkshopWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_CONTACT_EMAIL, TEST_NAME, TEST_DISPLAY_NAME,
            TEST_PHONE, TEST_ADDRESS, TEST_CITY, TEST_POSTAL_CODE, TEST_COUNTRY, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors.Should().ContainKey(EField.Email.ToString());
        result.Error.ValidationErrors![EField.Email.ToString()].Should().Contain("Identity with given email already exists.");

        await _transactionalExecutor.DidNotReceiveWithAnyArgs().ExecuteAsync(default!, default);
    }

    [Fact]
    public async Task CreateWorkshopWithIdentityAsync_Should_ReturnValidationError_WhenIdentityCreationFails()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(Task.FromResult<Identity?>(null));

        var identityErrors = new[] { new IdentityError { Code = "TestError", Description = "Password too weak." } };

        _userManager.CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD)
            .Returns(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _workshopService.CreateWorkshopWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_CONTACT_EMAIL, TEST_NAME, TEST_DISPLAY_NAME,
            TEST_PHONE, TEST_ADDRESS, TEST_CITY, TEST_POSTAL_CODE, TEST_COUNTRY, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors.Should().ContainKey("IdentityErrors");
        result.Error.ValidationErrors!["IdentityErrors"].Should().Contain("Password too weak.");

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _workshopRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    [Fact]
    public async Task CreateWorkshopWithIdentityAsync_Should_ReturnInternalError_WhenDomainWorkshopCreationFails()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(Task.FromResult<Identity?>(null));

        _userManager.CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD)
             .Returns(call =>
             {
                 var identity = call.Arg<Identity>();
                 identity.Id = Guid.NewGuid(); // Nadanie ID
                 return IdentityResult.Success;
             });

        _workshopRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("DB error"));

        // Act
        var result = await _workshopService.CreateWorkshopWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_CONTACT_EMAIL, TEST_NAME, TEST_DISPLAY_NAME,
            TEST_PHONE, TEST_ADDRESS, TEST_CITY, TEST_POSTAL_CODE, TEST_COUNTRY, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.InternalServerError);

        await _workshopRepository.Received(1).AddAsync(Arg.Is<Workshop>(w => w.Email == TEST_EMAIL), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateWorkshopWithIdentityAsync_Should_ReturnSuccess_WhenCreationIsSuccessful()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(Task.FromResult<Identity?>(null));

        _userManager.CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD)
            .Returns(call =>
            {
                var identity = call.Arg<Identity>();
                identity.Id = Guid.NewGuid();
                return IdentityResult.Success;
            });

        // Act
        var result = await _workshopService.CreateWorkshopWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_CONTACT_EMAIL, TEST_NAME, TEST_DISPLAY_NAME,
            TEST_PHONE, TEST_ADDRESS, TEST_CITY, TEST_POSTAL_CODE, TEST_COUNTRY, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userManager.Received(1).CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD);
        await _workshopRepository.Received(1).AddAsync(Arg.Is<Workshop>(w => w.Email == TEST_EMAIL && w.Name == TEST_NAME), Arg.Any<CancellationToken>());
        await _workshopRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateWorkshopWithIdentityAsync_Should_ReturnSuccess_WhenEmailIsUnchanged()
    {
        // Arrange
        var command = CreateValidUpdateCommand(TEST_EMAIL); // Używamy starego emaila

        _workshopRepository.GetByIdAsync(_domainWorkshopId, Arg.Any<CancellationToken>()).Returns(_fakeWorkshop);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);

        // Act
        var result = await _workshopService.UpdateWorkshopWithIdentityAsync(
            _domainWorkshopId, command.Email, command.ContactEmail, command.Name,
            command.DisplayName, command.PhoneNumber, command.Address, command.City,
            command.PostalCode, command.Country, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());

        // Email się nie zmienił, nie wołamy Usermanagera
        await _userManager.DidNotReceiveWithAnyArgs().SetEmailAsync(default!, default!);

        // Weryfikujemy aktualizację domeny
        _workshopRepository.Received(1).UpdateAsync(Arg.Is<Workshop>(w =>
            w.Name == command.Name && w.ContactEmail == command.ContactEmail
        ), Arg.Any<CancellationToken>());
        await _workshopRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateWorkshopWithIdentityAsync_Should_ReturnSuccess_WhenEmailIsChanged()
    {
        // Arrange
        var newEmail = "nowy-login@workshop.com";
        var command = CreateValidUpdateCommand(newEmail);

        _workshopRepository.GetByIdAsync(_domainWorkshopId, Arg.Any<CancellationToken>()).Returns(_fakeWorkshop);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);

        _userManager.FindByEmailAsync(newEmail).Returns(Task.FromResult<Identity?>(null)); // Email wolny
        _userManager.SetEmailAsync(_fakeIdentity, newEmail).Returns(IdentityResult.Success);
        _userManager.SetUserNameAsync(_fakeIdentity, newEmail).Returns(IdentityResult.Success);

        // Act
        var result = await _workshopService.UpdateWorkshopWithIdentityAsync(
            _domainWorkshopId, command.Email, command.ContactEmail, command.Name,
            command.DisplayName, command.PhoneNumber, command.Address, command.City,
            command.PostalCode, command.Country, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        await _userManager.Received(1).SetEmailAsync(_fakeIdentity, newEmail);
        await _userManager.Received(1).SetUserNameAsync(_fakeIdentity, newEmail);
        _workshopRepository.Received(1).UpdateAsync(Arg.Is<Workshop>(w => w.Email == newEmail), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateWorkshopWithIdentityAsync_Should_ReturnNotFoundError_WhenWorkshopIsMissing()
    {
        // Arrange
        var command = CreateValidUpdateCommand(TEST_EMAIL);
        _workshopRepository.GetByIdAsync(_domainWorkshopId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Workshop?>(null));

        // Act
        var result = await _workshopService.UpdateWorkshopWithIdentityAsync(
            _domainWorkshopId, command.Email, command.ContactEmail, command.Name,
            command.DisplayName, command.PhoneNumber, command.Address, command.City,
            command.PostalCode, command.Country, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

    [Fact]
    public async Task UpdateWorkshopWithIdentityAsync_Should_ReturnValidationError_WhenNewEmailIsTaken()
    {
        // Arrange
        var newEmail = "taken@workshop.com";
        var command = CreateValidUpdateCommand(newEmail);
        var otherIdentity = Identity.Create(newEmail, EIdentityType.User); // Inne ID

        _workshopRepository.GetByIdAsync(_domainWorkshopId, Arg.Any<CancellationToken>()).Returns(_fakeWorkshop);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);
        _userManager.FindByEmailAsync(newEmail).Returns(otherIdentity); // Email zajęty

        // Act
        var result = await _workshopService.UpdateWorkshopWithIdentityAsync(
            _domainWorkshopId, command.Email, command.ContactEmail, command.Name,
            command.DisplayName, command.PhoneNumber, command.Address, command.City,
            command.PostalCode, command.Country, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors![EField.Email.ToString()].Should().Contain("Email (login) already in use.");
    }

    private UpdateWorkshopCommand CreateValidUpdateCommand(string newEmail) => new(
        newEmail, "new_contact@w.pl", "New Name", "NewDisplay", "999",
        "New Address", "New City", "99-999", "New Country");
}
