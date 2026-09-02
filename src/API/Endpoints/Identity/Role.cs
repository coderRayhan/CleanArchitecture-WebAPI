using API.Extensions;
using API.Infrastructure;
using Application.Common.Security;
using Application.Features.Identity.Roles.Commands;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Identity;

public class Role : EndpointGroupBase
{
    
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("Create", Create)
            .WithName("CreateRole")
            .Produces<string>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Admin.IdentityRoles.Create);
        
        group.MapPut("Update", Create)
            .WithName("UpdateRole")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.Admin.IdentityRoles.Edit);

        group.MapPut("AddOrRemovePermissions", AddOrRemovePermissions)
            .WithName("AddOrRemovePermissions")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Admin.IdentityRoles.Create);
    }

    private async Task<IResult> Create(
        ISender sender,
        CreateRoleCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("CreateRole", new{id = result.Value}),
            onFailure: result.ToProblemDetails);
    }
    
    private async Task<IResult> Update(
        ISender sender,
        UpdateRoleCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> AddOrRemovePermissions(
        ISender sender,
        [FromBody] AddOrRemovePermissionCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: result.ToProblemDetails);
    }
}