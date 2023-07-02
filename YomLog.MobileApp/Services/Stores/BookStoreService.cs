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
    private readonly List<BookDetailsDTO> _booksCache = new();
    public ReadOnlyReactiveCollection<BookDetailsDTO> BookList { get; }

    public void Insert(BookDetailsDTO item)
    {
        _books.InsertOnScheduler(0, item);
        _booksCache.Insert(0, item);
    }

    public void Remove(BookDetailsDTO item)
    {
        _books.RemoveOnScheduler(item);
        _booksCache.Remove(item);
    }

    public void Edit(BookDetailsDTO item)
    {
        var currentItem = _books.FirstOrDefault(x => x == item);
        if (currentItem is null) return;

        int index = _books.IndexOf(currentItem);
        _books.SetOnScheduler(index, item);
        _booksCache[index] = item;
    }

    public void Set(List<BookDetailsDTO> items)
    {
        if (_books.Any())
        {
            _books.ClearOnScheduler();
            _booksCache.Clear();
        }

        _books.AddRangeOnScheduler(items);
        _booksCache.AddRange(items);
    }

    public BookDetailsDTO Get(Guid bookId) => _booksCache.First(x => x.Id == bookId);

    public BookDetailsDTO? GetOrDefault(Guid bookId) => _booksCache.FirstOrDefault(x => x.Id == bookId);
}
