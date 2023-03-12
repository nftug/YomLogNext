using YomLog.Shared.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.DTOs;

public interface IDetailsDTO : IDataTransferObject
{
    DateTime CreatedOn { get; init; }
    DateTime? UpdatedOn { get; init; }
    EntityReferenceWithName<User> CreatedBy { get; init; }
    EntityReferenceWithName<User>? UpdatedBy { get; init; }
}
