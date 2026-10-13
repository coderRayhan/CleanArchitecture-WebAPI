using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Application.Common.Security;
using Application.Features.MenuSectionSubItems.Commands;
using Domain.Shared;

namespace API.Endpoints.Setups;

public sealed class MenuSectionSubItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("Create", Create)
            .WithName("CreateMenuSectionSubItem")
            .Produces<Guid>(StatusCodes.Status201Created)
            .WithProblemResponses()
            .RequireAuthorization(Permissions.Admin.MenuSectionSubItems.Create);

        group.MapPut("Update", Update)
            .WithName("UpdateMenuSectionSubItem")
            .Produces(StatusCodes.Status200OK)
            .WithProblemResponses()
            .RequireAuthorization(Permissions.Admin.MenuSectionSubItems.Edit);

        group.MapDelete("Delete", Delete)
            .WithName("DeleteMenuSectionSubItem")
            .Produces(StatusCodes.Status204NoContent)
            .WithProblemResponses()
            .RequireAuthorization(Permissions.Admin.MenuSectionSubItems.Delete);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateMenuSectionSubItemCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetMenuSectionSubItem", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateMenuSectionSubItemCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteMenuSectionSubItemCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }
}
