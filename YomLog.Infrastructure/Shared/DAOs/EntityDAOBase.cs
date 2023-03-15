using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using YomLog.Shared.Entities;

namespace YomLog.Infrastructure.Shared.DAOs;

public abstract class EntityDAOBase<TEntity, TEntityDAO> : IEntityDAO
    where TEntity : EntityBase<TEntity>
    where TEntityDAO : EntityDAOBase<TEntity, TEntityDAO>, new()
{
    [Key] public long PK { get; set; }

    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public Guid? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }

    internal virtual TEntityDAO Transfer(TEntity origin)
    {
        Id = origin.Reference.Id;
        CreatedOn = origin.DateTimeRecord.CreatedOn;
        CreatedByName = origin.UserRecord.CreatedBy.Name;
        CreatedById = origin.UserRecord.CreatedBy.Id;
        UpdatedOn = origin.DateTimeRecord.UpdatedOn;
        UpdatedByName = origin.UserRecord.UpdatedBy?.Name;
        UpdatedById = origin.UserRecord.UpdatedBy?.Id;

        return (TEntityDAO)this;
    }

    internal virtual void ApplyNavigation(TEntity model) { }

    protected abstract TEntity PrepareDomainEntity();

    internal TEntity ToDomain()
        => PrepareDomainEntity().Transfer(
            reference: new(PK, Id),
            dateTimeRecord: new(CreatedOn, UpdatedOn),
            userRecord: new(
                new(CreatedById, CreatedByName),
                UpdatedById != null ? new((Guid)UpdatedById, UpdatedByName!) : null
            )
        );

    public static void BuildEdm(ModelBuilder modelBuilder)
        => new TEntityDAO().OnBuildEdm(modelBuilder);

    protected static void SetupTableBase(ModelBuilder modelBuilder)
        => modelBuilder.Entity<TEntityDAO>().ToTable(typeof(TEntity).Name);

    protected static void SetupIndexBase(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<TEntityDAO>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<TEntityDAO>().HasIndex(x => x.Id);
    }

    protected virtual void OnBuildEdm(ModelBuilder modelBuilder)
    {
        SetupTableBase(modelBuilder);
        SetupIndexBase(modelBuilder);
    }

    public override bool Equals(object? obj) => Id == (obj as TEntityDAO)?.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
