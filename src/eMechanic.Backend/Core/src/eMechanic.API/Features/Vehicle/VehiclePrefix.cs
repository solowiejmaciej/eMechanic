namespace eMechanic.API.Features.Vehicle;

public static class VehiclePrefix
{
    public const string ENDPOINT = "/vehicles";
    public const string TAG = "Vehicles";

    public const string CREATE = ENDPOINT;
    public const string GET_ALL = ENDPOINT;
    public const string GET_BY_ID = ENDPOINT + "/{id:guid}";
    public const string UPDATE = ENDPOINT + "/{id:guid}";
    public const string DELETE = ENDPOINT + "/{id:guid}";

    public static class Timeline
    {
        public const string GET = ENDPOINT + "/{id:guid}/timeline";
    }
}
