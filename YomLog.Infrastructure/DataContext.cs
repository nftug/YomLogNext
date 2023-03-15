using Microsoft.EntityFrameworkCore;
using YomLog.Infrastructure.EDMs;

namespace YomLog.Infrastructure;

public class DataContext : DbContext
{
    private string DataSource { get; set; } = string.Empty;

    public DataContext() { }

    public DataContext(string appDataPath)
    {
        DataSource = Path.Combine(appDataPath, "YomLog.db");
    }

    public DbSet<BookEDM> Books { get; set; } = null!;
    public DbSet<AuthorEDM> Authors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BookEDM.BuildEdm(modelBuilder);
        AuthorEDM.BuildEdm(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(
            $"Data Source={DataSource}",
            opt => opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        );
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
