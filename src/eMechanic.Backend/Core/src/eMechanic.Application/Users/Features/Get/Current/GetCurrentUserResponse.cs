namespace eMechanic.Application.Users.Features.Get.Current;

public record GetCurrentUserResponse(Guid Id, string FirstName, string LastName, string Email, string? PhoneNumber, DateTime CreatedAt);
