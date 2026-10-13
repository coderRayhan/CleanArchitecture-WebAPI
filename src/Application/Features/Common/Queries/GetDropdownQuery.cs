using Application.Common.Abstractions.Contracts;
using Application.Common.Models;

namespace Application.Features.Common.Queries;

public sealed record GetDropdownQuery(
    string Sql,
    object Parameter,
    string Key,
    bool? AllowCache = null)
    : ICacheableQuery<IEnumerable<DropdownModel>>
{
    public string CacheKey => Key;
    public TimeSpan? Expiration => null;
    public bool? AllowCache => AllowCache ?? true;
}