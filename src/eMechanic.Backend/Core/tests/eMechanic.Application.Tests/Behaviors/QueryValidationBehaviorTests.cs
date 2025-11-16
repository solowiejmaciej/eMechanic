namespace eMechanic.Application.Tests.Behaviors;

using eMechanic.Application.Behaviors;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;
using MediatR;
using NSubstitute;

public sealed record TestQuery(string Email) : IResultQuery<Guid>;

public class TestQueryValidator : AbstractValidator<TestQuery>
{
    public TestQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class QueryValidationBehaviorTests
{
    private readonly IValidator<TestQuery> _validator;
    private readonly RequestHandlerDelegate<Result<Guid, Error>> _nextDelegate;
    private readonly QueryValidationBehavior<TestQuery, Result<Guid, Error>> _behavior;

    public QueryValidationBehaviorTests()
    {
        _validator = new TestQueryValidator();

        _nextDelegate = Substitute.For<RequestHandlerDelegate<Result<Guid, Error>>>();

        _behavior = new QueryValidationBehavior<TestQuery, Result<Guid, Error>>(
            new[] { _validator }
        );
    }

    [Fact]
    public async Task Handle_Should_CallNext_WhenValidationIsSuccessful()
    {
        // Arrange
        var query = new TestQuery("poprawny@email.pl");
        var successResult = Guid.NewGuid();

        _nextDelegate.Invoke().Returns(successResult);

        // Act
        var result = await _behavior.Handle(query, _nextDelegate, CancellationToken.None);

        // Assert
        await _nextDelegate.Received(1).Invoke();

        Assert.False(result.HasError());
        Assert.Equal(successResult, result.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenValidationFails()
    {
        // Arrange
        var query = new TestQuery("this-is-not-a-valid-email");

        // Act
        var result = await _behavior.Handle(query, _nextDelegate, CancellationToken.None);

        // Assert
        await _nextDelegate.DidNotReceive().Invoke();

        Assert.True(result.HasError());
        Assert.Equal(EErrorCode.ValidationError, result.Error!.Code);
    }
}
