namespace Application.Common.Abstractions.Contracts;

public interface IIdempotentCommand
{
    string IdempotencyKey { get; }
}