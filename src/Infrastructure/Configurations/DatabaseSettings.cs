using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configurations;
public class DatabaseSettings : IValidatableObject
{
    /// <summary>
    ///     Represents the database provider, which to connect to
    /// </summary>
    public string DBProvider { get; set; } = string.Empty;

    /// <summary>
    ///     The connection string being used to connect with the given database provider
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    ///     The redis connection string for distributed caching system
    /// </summary>
    public string RedisConnection { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(DBProvider))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(DBProvider)} is not configured",  [nameof(ConnectionString)]);
        }

        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured", [nameof(ConnectionString)]);
        }
    }
}
