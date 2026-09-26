using API.Extensions;
using API.Infrastructure;
using Application.Common.Security;
using Application.Features.MenuSectionSubItems.Queries;
using MediatR;

namespace API.Endpoints.Setups;

public sealed class MenuItem : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("GetMenuItem", GetMenuItem)
            .WithName("GetMenuItem")
            .Produces<List<SideBar>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization(Permissions.Admin.MenuItems.View);
        
    }
    
    private async Task<IResult> GetMenuItem(ISender sender)
    {
        var result = await sender.Send(new GetMenuItemQuery());

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(result.Error);
    }
}