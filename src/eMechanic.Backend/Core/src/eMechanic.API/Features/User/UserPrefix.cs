namespace eMechanic.API.Features.User;

public static class UserPrefix
{
    public const string ENDPOINT = "/users";
    public const string TAG = "Users";

    public const string GET_CURRENT_USER_ENDPOINT = ENDPOINT + "/me";
    public const string CREATE_USER_ENDPOINT = ENDPOINT;
    public const string UPDATE_CURRENT_USER_ENDPOINT = ENDPOINT;
}
