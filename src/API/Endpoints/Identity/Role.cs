using API.Extensions;
using API.Infrastructure;
using Application.Features.Identity.Commands;
using Domain.Shared;
using MediatR;
using MyNamespace;

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
            .RequireAuthorization();
        
        group.MapPut("Update", Create)
            .WithName("UpdateRole")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
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
}