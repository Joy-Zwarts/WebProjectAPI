using Microsoft.EntityFrameworkCore;

namespace WebsiteAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasKey(project => project.Id);

        modelBuilder.Entity<Project>()
            .Property(project => project.Id)
            .ValueGeneratedOnAdd();
    }
}