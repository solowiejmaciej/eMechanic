namespace eMechanic.API.Features.Vehicle;

public static class VehiclePrefix
{
    public const string ENDPOINT = "/vehicles";
    public const string TAG = "Vehicles";

    public const string CREATE_ENDPOINT = ENDPOINT;
    public const string GET_ALL_ENDPOINT = ENDPOINT;
    public const string GET_BY_ID_ENDPOINT = ENDPOINT + "/{id:guid}";
    public const string UPDATE_ENDPOINT = ENDPOINT + "/{id:guid}";
    public const string DELETE_ENDPOINT = ENDPOINT + "/{id:guid}";
    public const string GET_TIMELINE_ENDPOINT = ENDPOINT + "/{id:guid}/timeline";
}
