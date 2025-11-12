namespace eMechanic.API.Features.Tokens;

public static class TokenPrefix
{
    public const string ENDPOINT = "/tokens";
    public const string TAG = "Token";

    public const string CREATE_USER_TOKEN_ENDPOINT = ENDPOINT + "/user";
    public const string CREATE_WORKSHOP_TOKEN_ENDPOINT = ENDPOINT + "/workshop";
    public const string REFRESH_TOKEN_ENDPOINT = ENDPOINT + "/refresh";
}
