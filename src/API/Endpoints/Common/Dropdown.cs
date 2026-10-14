using API.Infrastructure;
using API.Extensions;
using Application.Common.Abstractions.Caching;
using Application.Common.Constants;
using Application.Common.Models;
using Application.Features.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Common;

public class Dropdown : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("GetMenuSectionsDropdownList", GetLookupSelectList)
            .WithName("GetMenuSectionsDropdownList")
            .Produces<List<DropdownModel>>(StatusCodes.Status200OK);
    }
    
    private async Task<List<DropdownModel>> GetLookupSelectList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetDropdownQuery(
            Sql: DropdownSqlConstants.MenuSectionDropdownSql,
            Parameters: new { })
        );
        return result.Value;
    }
}