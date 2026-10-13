using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Domain.Shared;
using Mapster;

namespace Application.Features.MenuSectionItems.Queries;

public sealed record GetMenuSectionItemByIdQuery(Guid Id) : ICacheableQuery<MenuSectionItemResponse>
{
    public string CacheKey => $"MenuSectionItem_{Id}";
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetMenuSectionItemByIdQueryHandler(
    IApplicationDbContext context)
    : IQueryHandler<GetMenuSectionItemByIdQuery, MenuSectionItemResponse>
{
    public async Task<Result<MenuSectionItemResponse>> Handle(GetMenuSectionItemByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await context.MenuSectionItems.FindAsync(request.Id, cancellationToken);
        if (data is null)
            return Result.Failure<MenuSectionItemResponse>(Error.NotFound(nameof(request), "Menu Section Item not found."));

        return Result.Success(data.Adapt<MenuSectionItemResponse>());
    }
}

