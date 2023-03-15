using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace YomLog.Infrastructure.Shared.EDMs;

public abstract class JoinTableEDMBase<TSelf> : IJoinTableEDM
    where TSelf : JoinTableEDMBase<TSelf>
{
    [Key] public long PK { get; set; }

    public static void BuildEdm(ModelBuilder modelBuilder)
    {
        var tableName = typeof(TSelf).Name.Split("EDM").First();
        modelBuilder.Entity<TSelf>().ToTable(tableName);
    }
}
