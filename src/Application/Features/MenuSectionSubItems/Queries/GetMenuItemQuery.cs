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
                    ms.Title Tittle,
                    ms.Href Route,
                    ms.Icon,
                    REPLACE(ms.Href, '/', '') Base,
                    ms.HasSubRoute,
                    ms.HasSubRoute HasSubRouteTwo,
                    0 ShowSubRoute,
                    1 Dot,
                    'start' Materialicons
                   FROM MenuSections ms 

                   SELECT
                    a.Id,
                    a.MenuSectionId,
                    a.Title MenuValue,
                    a.Href Route,
                    REPLACE(a.Href, '/', '') Base,
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

        var sideBars = (await result.ReadAsync<SideBar>()).ToList();
        var sideBarMenus = (await result.ReadAsync<SideBarMenu>()).ToList();
        var subMenus = (await result.ReadAsync<SubMenu>()).ToList();

        // Group sub-menus under their parent menu item
        var subMenusByParent = subMenus
            .GroupBy(s => s.MenuSectionItemId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var menu in sideBarMenus)
        {
            menu.SubMenus = subMenusByParent.TryGetValue(menu.Id, out var subs)
                ? subs
                : [];
        }

        // Group menu items under their parent section
        var menusByParent = sideBarMenus
            .GroupBy(m => m.MenuSectionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var sideBar in sideBars)
        {
            sideBar.Menu = menusByParent.TryGetValue(sideBar.Id, out var menus)
                ? menus
                : [];
        }

        return Result.Success(sideBars);
    }
}