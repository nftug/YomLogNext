using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.Domain.Books.DTOs;
using YomLog.Shared.Attributes;

namespace YomLog.MobileApp.Services.Stores;

[InjectAsScoped]
public class BookStoreService : BindableBase
{
    public BookStoreService()
    {
        _books = new ReactiveCollection<BookDetailsDTO>().AddTo(Disposable);
        BookList = _books.ToReadOnlyReactiveCollection();
    }

    private readonly ReactiveCollection<BookDetailsDTO> _books;
    public ReadOnlyReactiveCollection<BookDetailsDTO> BookList { get; }

    public void Insert(BookDetailsDTO item) => _books.InsertOnScheduler(0, item);

    public void Remove(BookDetailsDTO item) => _books.RemoveOnScheduler(item);

    public void Edit(BookDetailsDTO item)
    {
        var currentItem = _books.FirstOrDefault(x => x == item);
        if (currentItem is null) return;
        _books.SetOnScheduler(_books.IndexOf(currentItem), item);
    }

    public void Set(List<BookDetailsDTO> items)
    {
        if (_books.Any()) _books.ClearOnScheduler();
        _books.AddRangeOnScheduler(items);
    }
}
