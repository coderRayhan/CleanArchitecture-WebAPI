using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Dapper;
using Domain.Shared;

namespace Application.Features.MenuSectionSubItems.Queries;

public sealed record GetMenuItemQuery : IQuery<List<SideBar>>;

internal sealed class GetMenuItemQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetMenuItemQuery, List<SideBar>>
{
    public async Task<Result<List<SideBar>>> Handle(GetMenuItemQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnectionFactory.GetOpenConnection();

        var sql = $"""
                   SELECT
                    ms.Id,
                    ms.Title MenuValue,
                    ms.Href Route,
                    ms.Icon,
                    REPLACE(ms.Href, '/', '') Base,
                    ms.HasSubRoute,
                    ms.HasSubRoute HasSubRouteTwo,
                    ms.HasSubRoute CustomSubmenuTwo,
                    0 ShowSubRoute,
                    1 Dot,
                    'start' Materialicons
                   FROM MenuSections ms 
                   
                   SELECT
                    a.Id,
                    a.MenuSectionId,
                    a.Title MenuValue,
                    a.Href Route,
                    null Base,
                    1 HasSubRoute,
                    0 ShowSubRoute,
                    1 CustomSubmenuTwo
                   FROM MenuSectionItems a
                   
                   SELECT
                    a.Id,
                    a.MenuSectionItemId,
                    a.Title MenuValue,
                    a.Href Route,
                    REPLACE(a.Href, '/', '') Base,
                    0 HasSubRoute,
                    0 ShowSubRoute
                   FROM MenuSectionSubItems a
                   """;
        using var result = await connection.QueryMultipleAsync(sql);

        var sideBarMenus = (await result.ReadAsync<SideBarMenu>()).ToList();
        var subMenus = (await result.ReadAsync<SubMenu>()).ToList();
        var subMenusTwo = (await result.ReadAsync<SubMenuTwo>()).ToList();

        // Level 3: SubMenuTwo grouped by its exact parent SubMenu.Id
        var subMenusTwoBySubMenuId = subMenusTwo
            .GroupBy(s => s.MenuSectionItemId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var subMenu in subMenus)
        {
            subMenu.SubMenusTwo = subMenusTwoBySubMenuId.TryGetValue(subMenu.Id, out var subsTwo)
                ? subsTwo
                : [];
        }

        // Level 2: SubMenu grouped under its parent SideBarMenu.Id
        var subMenusByMenuId = subMenus
            .GroupBy(s => s.MenuSectionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var menu in sideBarMenus)
        {
            menu.SubMenus = subMenusByMenuId.TryGetValue(menu.Id, out var subs)
                ? subs
                : [];
        }

        // Level 1: single SideBar root, all SideBarMenu rows bound directly
        var sideBarList = new List<SideBar>()
        {
            new SideBar()
            {
                Menu = sideBarMenus,
                Tittle = "",
                Icon = "airplay",
                ShowAsTab = true,
                SeparateRoute = false
            }
        };


        return Result.Success(sideBarList);
    }
}