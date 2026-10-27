using System.Text.Json.Serialization;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.DapperQueries;
using Application.Features.Identity.AppUsers.Models;
using Domain.Shared;

namespace Application.Features.Identity.AppUsers.Queries;

public sealed record GetAppUserListQuery
    : DataGridModel, ICacheableQuery<PaginatedResponse<AppUserModel>>
{
    [JsonIgnore]
    public string CacheKey => $"AppUser:{PageNumber}:{PageSize}";
}

internal sealed class GetAppUserListQueryHandler(
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<GetAppUserListQuery, PaginatedResponse<AppUserModel>>
{
    public async Task<Result<PaginatedResponse<AppUserModel>>> Handle(GetAppUserListQuery request, CancellationToken cancellationToken)
    {
        var connection = connectionFactory.GetOpenConnection();
        var sql = """
                  SELECT
                      U.Id AS Id, 
                      U.FirstName AS FirstName, 
                      U.LastName AS LastName, 
                      U.Username AS Username, 
                      U.Email AS Email, 
                      U.PhoneNumber AS PhoneNumber,
                      IIF(U.IsActive = 1, 'Active', 'Inactive') AS Status,
                      U.PhotoUrl AS PhotoUrl,
                      COALESCE(STRING_AGG(R.Name, ', '), '') AS AssignedRoles
                  FROM [identity].Users AS U
                  LEFT JOIN [identity].UserRoles AS UR ON U.Id = UR.UserId
                  LEFT JOIN [identity].Roles AS R ON UR.RoleId = R.Id
                  GROUP BY U.Id, U.FirstName, U.LastName, U.UserName, U.Email, U.Email, U.PhotoUrl, U.PhoneNumber, U.IsActive 
                  """;

        return await PaginatedResponse<AppUserModel>.CreateAsync(
            connection, 
            sql, 
            request);
    }
}