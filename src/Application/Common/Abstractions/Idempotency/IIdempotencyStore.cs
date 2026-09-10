namespace Application.Common.Abstractions.Idempotency;

public interface IIdempotencyStore
{
    Task<(IdempotencyCheckResult Result, IdempotencyRecord? Record)> TryBeginAsync(
        string key,
        string requestHash,
        TimeSpan ttl, 
        CancellationToken ct = default);
    
    Task CompleteAsync(
        string key,
        int statusCode,
        string? responseBody,
        CancellationToken ct = default);
    
    Task ReleaseAsync(
        string key,
        CancellationToken ct = default); // on failure, so it can be retried
}

public record IdempotencyRecord(int StatusCode, string? ResponseBody);

public enum IdempotencyCheckResult
{
    New,
    InProgress,
    Completed,
    KeyReusedWithDifferentPayload
}