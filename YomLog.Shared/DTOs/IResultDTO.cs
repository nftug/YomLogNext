using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.DTOs;

public interface IResultDTO : IDataTransferObject
{
    DateTime CreatedOn { get; init; }
    DateTime UpdatedOn { get; init; }
    UserReference CreatedBy { get; init; }
    UserReference UpdatedBy { get; init; }
}
