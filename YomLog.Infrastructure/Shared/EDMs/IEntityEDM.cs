namespace YomLog.Infrastructure.Shared.EDMs;

public interface IEntityEDM
{
    long PK { get; set; }
    Guid Id { get; set; }
    DateTime CreatedOn { get; set; }
    DateTime? UpdatedOn { get; set; }
    Guid CreatedById { get; set; }
    string CreatedByName { get; set; }
    Guid? UpdatedById { get; set; }
    string? UpdatedByName { get; set; }
}

public interface IJoinTableEDM
{
    long PK { get; set; }
}
