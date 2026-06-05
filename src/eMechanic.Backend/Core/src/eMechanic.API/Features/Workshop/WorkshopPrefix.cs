namespace eMechanic.API.Features.Workshop;

public static class WorkshopPrefix
{
    public const string ENDPOINT = "/workshops";
    public const string TAG = "Workshop";

    public const string CREATE_ENDPOINT = ENDPOINT;
    public const string GET_ALL_ENDPOINT = ENDPOINT;
    public const string UPDATE_ENDPOINT = ENDPOINT;
    public const string WORKSHOP_REVIEWS_ENDPOINT = $"{ENDPOINT}/{{workshopId}}/reviews";
    public const string GET_WORKSHOP_REVIEWS_ENDPOINT = WORKSHOP_REVIEWS_ENDPOINT;
    public const string UPSERT_WORKSHOP_REVIEW_ENDPOINT = WORKSHOP_REVIEWS_ENDPOINT;
    public const string DELETE_WORKSHOP_REVIEW_ENDPOINT = WORKSHOP_REVIEWS_ENDPOINT;
    public const string GET_WORKSHOP_REVIEW_STATS_ENDPOINT = $"{WORKSHOP_REVIEWS_ENDPOINT}/stats";
}
