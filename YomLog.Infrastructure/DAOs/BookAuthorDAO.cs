using YomLog.Infrastructure.Shared.DAOs;

namespace YomLog.Infrastructure.DAOs;

public class BookAuthorDAO : JoinTableDAOBase<BookAuthorDAO>
{
    public long FKBook { get; set; }
    public long FKAuthor { get; set; }

    public BookDAO Book { get; set; } = null!;
    public AuthorDAO Author { get; set; } = null!;
}
