using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Domain.Shared;
using Mapster;

namespace Application.Features.MenuSectionSubItems.Queries;

public sealed record GetMenuSectionSubItemByIdQuery(Guid Id) : ICacheableQuery<MenuSectionSubItemResponse>
{
    public string CacheKey => $"MenuSectionSubItems_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => null;
}

internal sealed class GetMenuSectionSubItemByIdQueryHandler(
    IApplicationDbContext context)
    : IQueryHandler<GetMenuSectionSubItemByIdQuery, MenuSectionSubItemResponse>
{
    public async Task<Result<MenuSectionSubItemResponse>> Handle(GetMenuSectionSubItemByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await context.MenuSectionSubItems.FindAsync(request.Id, cancellationToken);
        if (data is null)
            return Result.Failure<MenuSectionSubItemResponse>(
                Error.NotFound(nameof(request), "Menu Item not found."));

        return Result.Success(data.Adapt<MenuSectionSubItemResponse>());
    }
}
