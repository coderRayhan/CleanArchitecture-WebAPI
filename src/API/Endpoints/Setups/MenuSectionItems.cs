using API.Extensions;
using API.Infrastructure;
using Application.Common.DapperQueries;
using Application.Common.Security;
using Application.Features.MenuSectionItems.Commands;
using Application.Features.MenuSectionItems.Queries;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Setups;

public sealed class MenuSectionItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
            .WithName("GetMenuSectionItems")
            .Produces<PaginatedResponse<MenuSectionItemResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Admin.MenuSectionItems.View);

        group.MapGet("GetMenuSectionItem/{id:Guid}", Get)
            .WithName("GetMenuSectionItem")
            .Produces<MenuSectionItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Admin.MenuSectionItems.View);

        group.MapPost("Create", Create)
            .WithName("CreateMenuSectionItem")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Admin.MenuSectionItems.Create);

        group.MapPut("Update", Update)
            .WithName("UpdateMenuSectionItem")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.Admin.MenuSectionItems.Edit);

        group.MapDelete("Delete", Delete)
            .WithName("DeleteMenuSectionItem")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Admin.MenuSectionItems.Delete);
    }

    private async Task<IResult> GetAll(ISender sender, 
        [FromBody] GetMenuSectionItemsListQuery query,
        CancellationToken ct = default)
    {
        var result = await sender.Send(query, ct);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMenuSectionItemByIdQuery(id), ct);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateMenuSectionItemCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetMenuSectionItem", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateMenuSectionItemCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteMenuSectionItemCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }
}
