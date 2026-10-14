using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.Models;
using Dapper;
using Domain.Shared;

namespace Application.Features.Common.Queries;

public sealed record GetDropdownQuery(
    string Sql,
    object Parameters)
    : IQuery<List<DropdownModel>>
{
    // public string CacheKey => Key;
    // public TimeSpan? Expiration => null;
    // public bool? AllowCache => AllowCache ?? true;
}

internal sealed class GetDropdownQueryHandler(
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<GetDropdownQuery, List<DropdownModel>>
{
    public async Task<Result<List<DropdownModel>>> Handle(GetDropdownQuery request, CancellationToken cancellationToken)
    {
        var connection = connectionFactory.GetOpenConnection();
        var selectList = await connection
            .QueryAsync<DropdownModel>(request.Sql, request.Parameters);

        return Result.Success(selectList.AsList());
    }
}