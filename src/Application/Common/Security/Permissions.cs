namespace Application.Common.Security;
public static class Permissions
{
    /// <summary>
    /// These will generate constant property value of perticular module like Faculties
    /// </summary>
    /// <param name="module">The name of the module</param>
    /// <returns></returns>
    
    // Every permission claim uses this type.
    public const string ClaimType = "permission";

    // Policy names starting with this prefix get a policy generated on demand.
    public const string Prefix = "Permissions.";
    public static List<string> GeneratePermissionsForModule(string module)
    {
        return new List<string>
        {
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete"
        };
    }

    public static List<string> GetPermissionsForModules(List<string> modules)
    {
        List<string> permissions = [];
        foreach (string module in modules)
        {
            permissions.AddRange(GeneratePermissionsForModule(module));
        }
        return permissions;
    }

    /// <summary>
    /// These will generate list of modules name
    /// </summary>
    /// <returns>List of string</returns>
    public static List<string> GetAllNestedModule(Type type)
    {
        Type[] nestedTypes = type.GetNestedTypes();
        List<string> result = [];
        foreach (var nestedType in nestedTypes)
        {
            result.Add(nestedType.Name);
        }
        return result;
    }

    /// <summary>
    /// The will generate list of modules type
    /// </summary>
    /// <returns>Array of Type</returns>
    public static Type[] GetAllNestedModuleType()
    {
        Type permissionType = typeof (Permissions);
        Type[] nestedTypes = permissionType.GetNestedTypes();
        return nestedTypes;
    }

    public static class Admin
    {
        public static class ApplicationUsers
        {
            public const string View = "Permissions.ApplicationUsers.View";
            public const string Create = "Permissions.ApplicationUsers.Create";
            public const string Edit = "Permissions.ApplicationUsers.Edit";
            public const string Delete = "Permissions.ApplicationUsers.Delete";
        }
        
        public static class IdentityRoles
        {
            public const string View = "Permissions.IdentityRoles.View";
            public const string Create = "Permissions.IdentityRoles.Create";
            public const string Edit = "Permissions.IdentityRoles.Edit";
            public const string Delete = "Permissions.IdentityRoles.Delete";
        }
        
        public static class ManageUserRoles
        {
            public const string View = "Permissions.ManageUserRoles.View";
            public const string Create = "Permissions.ManageUserRoles.Create";
            public const string Edit = "Permissions.ManageUserRoles.Edit";
            public const string Delete = "Permissions.ManageUserRoles.Delete";
        }
        
        public static class ManageRoleClaims
        {
            public const string View = "Permissions.ManageRoleClaims.View";
            public const string Create = "Permissions.ManageRoleClaims.Create";
            public const string Edit = "Permissions.ManageRoleClaims.Edit";
            public const string Delete = "Permissions.ManageRoleClaims.Delete";
        }
    }

    public static class CommonSetup
    {
        public static class Lookups
        {
            public const string View = "Permissions.Lookups.View";
            public const string Create = "Permissions.Lookups.Create";
            public const string Edit = "Permissions.Lookups.Edit";
            public const string Delete = "Permissions.Lookups.Delete";
        }

        public static class LookupDetails
        {
            public const string View = "Permissions.LookupDetails.View";
            public const string Create = "Permissions.LookupDetails.Create";
            public const string Edit = "Permissions.LookupDetails.Edit";
            public const string Delete = "Permissions.LookupDetails.Delete";
        }
    }
}
