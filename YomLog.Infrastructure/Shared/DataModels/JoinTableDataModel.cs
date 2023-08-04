using System.ComponentModel.DataAnnotations;

namespace YomLog.Infrastructure.Shared.DataModels;

public abstract class JoinTableDataModel<TSelf> : IJoinTableDataModel
    where TSelf : JoinTableDataModel<TSelf>
{
    [Key] public long PK { get; set; }
}
