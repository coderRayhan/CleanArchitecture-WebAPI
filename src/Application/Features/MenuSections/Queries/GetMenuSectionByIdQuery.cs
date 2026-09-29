using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Domain.Shared;
using Mapster;

namespace Application.Features.MenuSections.Queries;

public sealed record GetMenuSectionByIdQuery(Guid Id)
    : ICacheableQuery<MenuSectionResponse>
{
    public string CacheKey => $"MenuSection_{Id}";
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetMenuSectionByIdQueryHandler(
    IApplicationDbContext context)
    : IQueryHandler<GetMenuSectionByIdQuery, MenuSectionResponse>
{
    public async Task<Result<MenuSectionResponse>> Handle(GetMenuSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await context.MenuSections.FindAsync(request.Id, cancellationToken);

        if (data is null)
            return Result.Failure<MenuSectionResponse>(Error.NotFound(nameof(request), ErrorMessages.EntityNotFound));
        
        return Result.Success(data.Adapt<MenuSectionResponse>());
    }
}