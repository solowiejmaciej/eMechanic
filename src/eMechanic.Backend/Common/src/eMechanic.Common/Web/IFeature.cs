namespace eMechanic.Common.Web;

using eMechanic.Common.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public interface IFeature
{
    IResult MapError(Error error);
    void MapEndpoint(IEndpointRouteBuilder app);
}
