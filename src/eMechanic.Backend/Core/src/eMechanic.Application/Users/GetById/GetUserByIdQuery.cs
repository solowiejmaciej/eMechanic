namespace eMechanic.Application.Users.GetById;

using Common.CQRS;
using FluentValidation;

public sealed record GetUserByIdQuery(Guid Id) : IResultQuery<GetUsersByIdResponse>;

public class GetUsersByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUsersByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("User id cannot be empty");
    }
}
