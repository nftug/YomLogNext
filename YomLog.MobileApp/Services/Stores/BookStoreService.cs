using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.Domain.Books.Entities;

namespace YomLog.MobileApp.Services.Stores;

public class BookStoreService : BindableBase
{
    public BookStoreService()
    {
        Books = new ReactiveCollection<Book>().AddTo(Disposable);
        BookList = Books.ToReadOnlyReactiveCollection();
    }

    private ReactiveCollection<Book> Books { get; set; }
    public ReadOnlyReactiveCollection<Book> BookList { get; }

    public void Insert(Book item) => Books.InsertOnScheduler(0, item);

    public void Bind(List<Book> items)
    {
        if (Books.Any()) Books.ClearOnScheduler();
        Books.AddRangeOnScheduler(items);
    }

    public void Remove(Book item) => Books.RemoveOnScheduler(item);
}
