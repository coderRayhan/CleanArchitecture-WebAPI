using System.Text.Json.Serialization;

namespace Application.Common.Abstractions.Contracts;
public interface ICacheableQuery
{
    [JsonIgnore]
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
    bool? AllowCache { get; }
}

public interface ICacheableQuery<TResponse> : ICacheableQuery, IQuery<TResponse>;
