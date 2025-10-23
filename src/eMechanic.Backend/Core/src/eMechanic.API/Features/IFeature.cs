namespace eMechanic.API.Features;

using Common.Result;

public interface IFeature
{
    IResult MapError(Error error);
    void MapEndpoint(IEndpointRouteBuilder app);
}
