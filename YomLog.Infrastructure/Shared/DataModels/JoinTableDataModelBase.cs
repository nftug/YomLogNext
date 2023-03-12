using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace YomLog.Infrastructure.Shared.DataModels;

public abstract class JoinTableDataModelBase<TSelf> : IJoinTableDataModel
    where TSelf : JoinTableDataModelBase<TSelf>
{
    [Key] public long PK { get; set; }

    public static void BuildEdm(ModelBuilder modelBuilder)
    {
        var tableName = typeof(TSelf).Name.Split("DataModel").First();
        modelBuilder.Entity<TSelf>().ToTable(tableName);
    }
}
