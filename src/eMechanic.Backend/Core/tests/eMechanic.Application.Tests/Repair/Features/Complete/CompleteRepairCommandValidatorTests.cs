namespace eMechanic.Application.Tests.Repair.Features.Complete;

using Application.Repair.Features.Complete;
using FluentValidation.TestHelper;

public class CompleteRepairCommandValidatorTests
{
    private readonly CompleteRepairCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new CompleteRepairCommand(Guid.NewGuid(), 1250m, "PLN");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenAmountIsLessOrEqualZero()
    {
        var command = new CompleteRepairCommand(Guid.NewGuid(), 0m, "PLN");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_HaveError_WhenCurrencyLengthIsNotThree()
    {
        var command = new CompleteRepairCommand(Guid.NewGuid(), 100m, "PL");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }
}

