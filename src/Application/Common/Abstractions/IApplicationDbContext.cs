using Domain.Entities;
using Domain.Entities.SuperAdmin;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Abstractions;
public interface IApplicationDbContext
{
	#region Common Setup
	DbSet<Lookup> Lookups { get; }
	DbSet<LookupDetails> LookupDetails { get; }
	#endregion

	public DbSet<MenuSection> MenuSections { get; }
	public DbSet<MenuSectionItem> MenuSectionItems { get; }
	public DbSet<MenuSectionSubItem> MenuSectionSubItems { get; }
	public DbSet<IdempotencyKeyEntity> IdempotencyKeys { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
