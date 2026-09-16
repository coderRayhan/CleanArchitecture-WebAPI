using Application.Common.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.SuperAdmin;

namespace Infrastructure.Persistence;
public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        :base(options)
    {
        
    }
    public DbSet<Lookup> Lookups => Set<Lookup>();

    public DbSet<LookupDetails> LookupDetails => Set<LookupDetails>();
    public DbSet<MenuSection> MenuSections => Set<MenuSection>();
    public DbSet<MenuSectionItem> MenuSectionItems => Set<MenuSectionItem>();
    public DbSet<MenuSectionSubItem> MenuSectionSubItems => Set<MenuSectionSubItem>();
    public DbSet<IdempotencyKeyEntity> IdempotencyKeys => Set<IdempotencyKeyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
