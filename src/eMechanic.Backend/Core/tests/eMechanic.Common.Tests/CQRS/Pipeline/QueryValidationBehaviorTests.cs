namespace eMechanic.Common.Tests.CQRS.Pipeline;

using eMechanic.Common.CQRS;
using eMechanic.Common.CQRS.Pipeline;
using eMechanic.Common.Result;
using FluentValidation;
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
    private readonly Func<CancellationToken, Task<Result<Guid, Error>>> _nextDelegate;
    private readonly QueryValidationBehavior<TestQuery, Result<Guid, Error>> _behavior;

    public QueryValidationBehaviorTests()
    {
        _validator = new TestQueryValidator();
        _nextDelegate = Substitute.For<Func<CancellationToken, Task<Result<Guid, Error>>>>();

        _behavior = new QueryValidationBehavior<TestQuery, Result<Guid, Error>>([_validator]);
    }

    [Fact]
    public async Task Handle_Should_CallNext_WhenValidationIsSuccessful()
    {
        var query = new TestQuery("valid@email.com");
        var successResult = Guid.NewGuid();

        _nextDelegate.Invoke(Arg.Any<CancellationToken>()).Returns(successResult);

        var result = await _behavior.Handle(query, _nextDelegate, CancellationToken.None);

        await _nextDelegate.Received(1).Invoke(Arg.Any<CancellationToken>());
        Assert.False(result.HasError());
        Assert.Equal(successResult, result.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenValidationFails()
    {
        var query = new TestQuery("invalid-email");

        var result = await _behavior.Handle(query, _nextDelegate, CancellationToken.None);

        await _nextDelegate.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
        Assert.True(result.HasError());
        Assert.Equal(EErrorCode.ValidationError, result.Error!.Code);
    }
}

