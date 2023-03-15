using System.ComponentModel.DataAnnotations.Schema;
using YomLog.Infrastructure.Shared.EDMs;

namespace YomLog.Infrastructure.EDMs;

[Table("BookAuthor")]
public class BookAuthorEDM : JoinTableEDMBase<BookAuthorEDM>
{
    public long FKBook { get; set; }
    public long FKAuthor { get; set; }

    public BookEDM Book { get; set; } = null!;
    public AuthorEDM Author { get; set; } = null!;
}
