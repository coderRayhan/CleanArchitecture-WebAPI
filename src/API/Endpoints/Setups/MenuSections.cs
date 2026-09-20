using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Application.Common.Security;
using Application.Features.MenuSections.Commands;
using Domain.Shared;

namespace API.Endpoints.Setups;

public sealed class MenuSections : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("Create", Create)
            .WithName("CreateMenuSection")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
            // .RequireAuthorization(Permissions.SuperAdmin.MenuSections.Create);

        group.MapPut("Update", Update)
            .WithName("UpdateMenuSection")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
            // .RequireAuthorization(Permissions.SuperAdmin.MenuSections.Edit);

        group.MapDelete("Delete", Delete)
            .WithName("DeleteMenuSection")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
        // .RequireAuthorization(Permissions.SuperAdmin.MenuSections.Delete);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateMenuSectionCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetMenuSection", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateMenuSectionCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteMenuSectionCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }
}
