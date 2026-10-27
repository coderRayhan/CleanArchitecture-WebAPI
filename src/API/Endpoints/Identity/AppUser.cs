using API.Extensions;
using API.Infrastructure;
using Application.Common.DapperQueries;
using Application.Common.Security;
using Application.Features.Identity.AppUsers.Models;
using Application.Features.Identity.AppUsers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Identity;

public class AppUser : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
            .WithName("GetAll")
            .Produces<PaginatedResponse<AppUserModel>>(StatusCodes.Status200OK)
            .RequireAuthorization(Permissions.Admin.ApplicationUsers.View);
    }
    
    private async Task<IResult> GetAll(ISender sender, [FromBody] GetAppUserListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }
}