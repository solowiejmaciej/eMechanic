namespace eMechanic.Application.Users.GetById;

using Common.CQRS;
using FluentValidation;

public sealed record GetUserByIdQuery(Guid Id, string test) : IResultQuery<GetUsersByIdResponse>;

public class GetUsersByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUsersByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("User id cannot be empty");

        RuleFor(x => x.test)
            .Must(x => x.StartsWith('a'))
            .WithMessage("User id must start with 'a'");
    }
}
