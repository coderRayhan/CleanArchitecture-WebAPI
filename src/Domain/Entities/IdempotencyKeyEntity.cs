namespace Domain.Entities;

public class IdempotencyKeyEntity
{
    public string IdempotencyKey { get; set; } = default!;   // PK
    public string RequestHash { get; set; } = default!;      // SHA-256 of endpoint + body
    public string Status { get; set; } = default!;           // "Processing" | "Completed"
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}