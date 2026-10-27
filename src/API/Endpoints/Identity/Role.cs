using API.Extensions;
using API.Infrastructure;
using Application.Common.DapperQueries;
using Application.Common.Security;
using Application.Features.Identity.Roles.Commands;
using Application.Features.Identity.Roles.Models;
using Application.Features.Identity.Roles.Queries;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Identity;

public class Role : EndpointGroupBase
{
    
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);
        
        group.MapPost("GetAll", GetAll)
            .WithName("GetRoles")
            .Produces<PaginatedResponse<RoleModel>>(StatusCodes.Status200OK)
            .RequireAuthorization(Permissions.Admin.IdentityRoles.View);

        group.MapPost("Create", Create)
            .WithName("CreateRole")
            .Produces<string>(StatusCodes.Status201Created)
            .WithProblemResponses()
            .RequireAuthorization(Permissions.Admin.IdentityRoles.Create);
        
        group.MapPut("Update", Update)
            .WithName("UpdateRole")
            .Produces(StatusCodes.Status200OK)
            .WithProblemResponses()
            .RequireAuthorization(Permissions.Admin.IdentityRoles.Edit);
        
        group.MapPut("Delete/{id}", Delete)
            .WithName("DeleteRole")
            .Produces(StatusCodes.Status204NoContent)
            .WithProblemResponses()
            .RequireAuthorization(Permissions.Admin.IdentityRoles.Delete);

        group.MapGet("GetRolePermissions/{roleId}", GetRolePermissionsAsync)
            .WithName("GetRolePermissions")
            .Produces<RoleModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(Permissions.Admin.IdentityRoles.View);

        group.MapPut("AddOrRemovePermissions", AddOrRemovePermissions)
            .WithName("AddOrRemovePermissions")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Admin.IdentityRoles.Create);
    }
    
    private async Task<IResult> GetAll(ISender sender, [FromBody] GetRoleListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
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
    
    public async Task<IResult> Delete(ISender sender, [FromRoute] string id)
    {
        var result = await sender.Send(new DeleteRoleCommand(id));
        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> GetRolePermissionsAsync(ISender sender, [FromRoute] string roleId)
    {
        var result = await sender.Send(new GetRolePermissionsQuery(roleId));
        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
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