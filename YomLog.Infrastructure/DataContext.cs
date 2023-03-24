using Microsoft.EntityFrameworkCore;
using YomLog.Infrastructure.EDMs;

namespace YomLog.Infrastructure;

public class DataContext : DbContext
{
    public DataContext() { }

    public DataContext(string appDataPath)
    {
        DataSource = Path.Combine(appDataPath, "YomLog.db");
    }

    public DbSet<BookEDM> Books { get; set; } = null!;
    public DbSet<AuthorEDM> Authors { get; set; } = null!;
    public DbSet<ProgressEDM> Progress { get; set; } = null!;

    private string DataSource { get; set; } = string.Empty;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(
            $"Data Source={DataSource}",
            opt => opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        );
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BookEDM.BuildEdm(modelBuilder);
        AuthorEDM.BuildEdm(modelBuilder);
        ProgressEDM.BuildEdm(modelBuilder);
    }
}
