namespace eMechanic.Application.Users.Get.Current;

public record GetCurrentUserResponse(Guid Id, string FirstName, string LastName, string Email, DateTime CreatedAt);
