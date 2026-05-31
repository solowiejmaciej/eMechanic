namespace eMechanic.API.Features.Repair;

public static class RepairPrefix
{
    public const string TAG = "Repairs";
    public const string PREFIX = "/repairs";
    public const string GET_BY_ID = $"{PREFIX}/{{id}}";
    public const string START = $"{PREFIX}/{{id}}/start";
    public const string COMPLETE = $"{PREFIX}/{{id}}/complete";
    public const string PAY = $"{PREFIX}/{{id}}/pay";
}

