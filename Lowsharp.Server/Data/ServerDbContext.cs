using Microsoft.EntityFrameworkCore;

namespace Lowsharp.Server.Data;

internal sealed class ServerDbContext : DbContext
{
    public DbSet<JsonCacheItem> CacheItems { get; set; } = null!;

    public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<JsonCacheItem>().HasKey(c => c.Id);
    }
}
