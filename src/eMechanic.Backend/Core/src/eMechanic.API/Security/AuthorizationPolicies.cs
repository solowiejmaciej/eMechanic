namespace eMechanic.API.Security;

public static class AuthorizationPolicies
{
    public const string MUST_BE_USER = "MustBeUser";
    public const string MUST_BE_WORKSHOP = "MustBeWorkshop";
    public const string MUST_BE_USER_OR_WORKSHOP = "MustBeUserOrWorkshop";
}
