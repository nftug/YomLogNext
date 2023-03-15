using Microsoft.EntityFrameworkCore;
using YomLog.Infrastructure.DAOs;

namespace YomLog.Infrastructure;

public class DataContext : DbContext
{
    private string DataSource { get; set; } = string.Empty;

    public DataContext() { }

    public DataContext(string appDataPath)
    {
        DataSource = Path.Combine(appDataPath, "YomLog.db");
    }

    public DbSet<BookDAO> Books { get; set; } = null!;
    public DbSet<AuthorDAO> Authors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BookDAO.BuildEdm(modelBuilder);
        AuthorDAO.BuildEdm(modelBuilder);
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
