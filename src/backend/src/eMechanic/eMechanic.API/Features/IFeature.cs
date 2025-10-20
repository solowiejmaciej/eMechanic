namespace eMechanic.API.Features;

using Common.Result;

public interface IFeature
{
    void MapEndpoint(IEndpointRouteBuilder app);

    IResult MapError(Error error);
}
