using YomLog.Infrastructure.Shared.DataModels;

namespace YomLog.Infrastructure.DataModels;

public class BookAuthorDataModel : JoinTableDataModelBase<BookAuthorDataModel>
{
    public long FKBook { get; set; }
    public long FKAuthor { get; set; }

    public BookDataModel Book { get; set; } = null!;
    public AuthorDataModel Author { get; set; } = null!;
}
