using System.ComponentModel.DataAnnotations;

namespace YomLog.Infrastructure.Shared.EDMs;

public abstract class JoinTableEDMBase<TSelf> : IJoinTableEDM
    where TSelf : JoinTableEDMBase<TSelf>
{
    [Key] public long PK { get; set; }
}
