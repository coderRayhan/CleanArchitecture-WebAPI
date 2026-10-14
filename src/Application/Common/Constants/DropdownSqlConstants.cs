namespace Application.Common.Constants;

public static class DropdownSqlConstants
{
    public const string MenuSectionDropdownSql = $"""
                                                  SELECT
                                                  	MS.Id,
                                                  	MS.Title Name
                                                  FROM MenuSections MS
                                                  WHERE MS.IsActive = 1
                                                  """;
}