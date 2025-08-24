namespace Application.Common.Abstractions.Contracts;
public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
    bool? AllowCache { get; }
}

public interface ICacheableQuery<TResponse> : ICacheableQuery, IQuery<TResponse>;
