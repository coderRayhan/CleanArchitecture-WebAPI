namespace Application.Features.MenuSectionSubItems.Queries;

public class SubMenu
{
    public Guid MenuSectionItemId { get; set; }
    public string MenuValue { get; set; }
    public string Route { get; set; }
    public string Base { get; set; }
    public bool Dot { get; set; }
    public bool HasSubRoute { get; set; }
    public bool ShowSubRoute { get; set; }
}

public class SideBarMenu
{
    public Guid Id { get; set; }
    public Guid MenuSectionId { get; set; }
    public string MenuValue { get; set; }
    public string Route { get; set; }
    public string Icon { get; set; }
    public string Base { get; set; }
    public bool HasSubRoute { get; set; }
    public bool ShowSubRoute { get; set; }
    public bool CustomSubMenuTwo { get; set; }
    public List<SubMenu> SubMenus { get; set; }
}

public class SideBar
{
    public Guid Id { get; set; }
    public string Tittle { get; set; }
    public string Route { get; set; }
    public string Icon { get; set; }
    public string Base { get; set; }
    public string Materialicons { get; set; }
    public bool HasSubRoute { get; set; }
    public bool HasSubRouteTwo { get; set; }
    public bool ShowAsTab { get; set; }
    public bool SeparateRoute { get; set; }
    public List<SideBarMenu> Menu { get; set; }
}

