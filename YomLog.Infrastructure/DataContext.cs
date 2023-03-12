using Microsoft.EntityFrameworkCore;
using YomLog.Infrastructure.DataModels;

namespace YomLog.Infrastructure;

public class DataContext : DbContext
{
    private string DataSource { get; set; } = string.Empty;

    public DataContext() { }

    public DataContext(string appDataPath)
    {
        DataSource = Path.Combine(appDataPath, "YomLog.db");
    }

    public DbSet<BookDataModel> Books { get; set; } = null!;
    public DbSet<AuthorDataModel> Authors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BookDataModel.BuildEdm(modelBuilder);
        AuthorDataModel.BuildEdm(modelBuilder);
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
