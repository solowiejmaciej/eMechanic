namespace eMechanic.API.Features.Repair;

public static class RepairPrefix
{
    public const string TAG = "Repairs";
    public const string PREFIX = "/repairs";
    public const string GET_BY_ID_FOR_USER = $"{PREFIX}/user/{{id}}";
    public const string GET_BY_ID_FOR_WORKSHOP = $"{PREFIX}/workshop/{{id}}";
    public const string GET_FOR_USER = $"{PREFIX}/user";
    public const string GET_FOR_WORKSHOP = $"{PREFIX}/workshop";
    public const string START = $"{PREFIX}/{{id}}/start";
    public const string COMPLETE = $"{PREFIX}/{{id}}/complete";
}

