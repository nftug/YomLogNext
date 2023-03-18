using System.ComponentModel.DataAnnotations;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Shared.DTOs;

namespace YomLog.Domain.Books.Commands;

public class ProgressCommandDTO : ICommandDTO<Progress>
{
    [Required] public Guid BookId { get; set; }
    [Required] public ProgressState State { get; set; }
    public int? Page { get; set; }
    public int? KindleLocation { get; set; }
}
