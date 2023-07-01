using Microsoft.EntityFrameworkCore;
using YomLog.Infrastructure.EDMs;

namespace YomLog.Infrastructure;

public class DataContext : DbContext
{
    // Need for generating migration codes
    public DataContext() { }

    public DataContext(string appDataPath)
        => _dataSource = Path.Combine(appDataPath, "YomLog.db");

    public DbSet<BookEDM> Books { get; set; } = null!;
    public DbSet<AuthorEDM> Authors { get; set; } = null!;
    public DbSet<ProgressEDM> Progress { get; set; } = null!;

    private readonly string _dataSource = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(
            $"Data Source={_dataSource}",
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
