using Microsoft.EntityFrameworkCore;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Infrastructure.Shared.EDMs;

namespace YomLog.Infrastructure.EDMs;

public class ProgressEDM : EntityEDMBase<Progress, ProgressEDM>
{
    public long FKBook { get; set; }
    public int Page { get; set; }
    public int? KindleLocation { get; set; }
    public ProgressState State { get; set; }

    public BookEDM Book { get; set; } = null!;

    protected override Progress PrepareDomainEntity()
        => new(
            book: new(FKBook, Book.Id, new(Book.TotalPage, Book.TotalKindleLocation)),
            page: new(Page, KindleLocation),
            state: State
        );

    internal override ProgressEDM Transfer(Progress origin)
    {
        FKBook = origin.Book.PK;
        Page = origin.BookPage.Page.Value;
        KindleLocation = origin.BookPage.KindleLocation?.Value;
        State = origin.State;
        return base.Transfer(origin);
    }

    protected override void OnBuildEdm(ModelBuilder modelBuilder)
    {
        base.OnBuildEdm(modelBuilder);

        modelBuilder.Entity<ProgressEDM>()
            .HasOne(x => x.Book)
            .WithMany(x => x.Progress)
            .HasForeignKey(x => x.FKBook);
    }
}
