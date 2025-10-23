namespace eMechanic.Application.Users.GetById;

public record GetUsersByIdResponse(Guid Id, string FirstName, string LastName, string Email, DateTime CreatedAt);
