using Application.Common.Abstractions;
using Application.Common.Abstractions.Idempotency;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Idempotency;

public class SqlServerIdempotencyStore(IApplicationDbContext _dbContext) 
    : IIdempotencyStore
{
    public async Task<(IdempotencyCheckResult Result, IdempotencyRecord? Record)> TryBeginAsync(
        string key, 
        string requestHash, 
        TimeSpan ttl, 
        CancellationToken ct = default)
    {
        var existing = await _dbContext.IdempotencyKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdempotencyKey == key, ct);

        if (existing != null)
        {
            if (existing.ExpiresAt < DateTime.UtcNow)
            {
                // expired - clean up and treat as new
                await _dbContext.IdempotencyKeys
                    .Where(x => x.IdempotencyKey == key)
                    .ExecuteDeleteAsync(ct);
            }
            else if (existing.RequestHash != requestHash)
            {
                return (IdempotencyCheckResult.KeyReusedWithDifferentPayload, null);
            }
            else if (existing.Status == "Processing")
            {
                return (IdempotencyCheckResult.InProgress, null);
            }
            else
            {
                return (IdempotencyCheckResult.Completed,
                    new IdempotencyRecord(existing.StatusCode!.Value, existing.ResponseBody));
            }
        }

        var entity = new IdempotencyKeyEntity
        {
            IdempotencyKey = key,
            RequestHash = requestHash,
            Status = "Processing",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(ttl)
        };

        try
        {
            _dbContext.IdempotencyKeys.Add(entity);
            await _dbContext.SaveChangesAsync(ct);
            return (IdempotencyCheckResult.New, null);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // Lost the race to a concurrent identical request
            return (IdempotencyCheckResult.InProgress, null);
        }
    }

    public async Task CompleteAsync(
        string key, 
        int statusCode, 
        string? responseBody, 
        CancellationToken ct = default)
    {
        await _dbContext.IdempotencyKeys
            .Where(x => x.IdempotencyKey == key)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Status, "Completed")
                .SetProperty(x => x.StatusCode, statusCode)
                .SetProperty(x => x.ResponseBody, responseBody), ct);
    }

    public async Task ReleaseAsync(
        string key, 
        CancellationToken ct = default)
    {
        // Let a failed request be retried with the same key instead of stuck "Processing" forever
        await _dbContext.IdempotencyKeys
            .Where(x => x.IdempotencyKey == key)
            .ExecuteDeleteAsync(ct);
    }
    
    
    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601);
}