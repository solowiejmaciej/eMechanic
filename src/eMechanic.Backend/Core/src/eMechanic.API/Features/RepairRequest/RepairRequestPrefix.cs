
namespace eMechanic.API.Features.RepairRequest;

public static class RepairRequestPrefix
{
    public const string TAG = "RepairRequests";
    public const string PREFIX = "/repairs";
    public const string CREATE = PREFIX;
    public const string PROVIDE_ESTIMATION = $"{PREFIX}/{{id}}/estimation";
    public const string ACCEPT_ESTIMATION = $"{PREFIX}/{{id}}/accept";
    public const string REJECT_ESTIMATION = $"{PREFIX}/{{id}}/reject";
    public const string GET_BY_WORKSHOP_ID = $"{PREFIX}";
    public const string GET_BY_VEHICLE_ID = $"{PREFIX}/vehicle/{{vehicleId}}";
    public const string GET_BY_ID = $"{PREFIX}/{{id}}";
    public const string SUMMARIZE = $"{PREFIX}/{{id}}/summarize";
}
