using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace YomLog.Infrastructure.Shared.DAOs;

public abstract class JoinTableDAOBase<TSelf> : IJoinTableDAO
    where TSelf : JoinTableDAOBase<TSelf>
{
    [Key] public long PK { get; set; }

    public static void BuildEdm(ModelBuilder modelBuilder)
    {
        var tableName = typeof(TSelf).Name.Split("DAO").First();
        modelBuilder.Entity<TSelf>().ToTable(tableName);
    }
}
