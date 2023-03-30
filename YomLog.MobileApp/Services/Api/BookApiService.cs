using MediatR;
using MudBlazor;
using YomLog.BlazorShared.Services.Popup;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.MobileApp.Services.Stores;
using YomLog.Shared.Attributes;
using YomLog.Shared.Exceptions;
using YomLog.UseCase.Books;

namespace YomLog.MobileApp.Services.Api;

[InjectAsScoped]
public class BookApiService
{
    protected readonly ISender _mediator;
    protected readonly ISnackbar _snackbar;
    protected readonly IPopupService _popupService;
    protected readonly BookStoreService _store;

    public BookApiService(ISender mediator, ISnackbar snackbar, IPopupService popupService, BookStoreService store)
    {
        _mediator = mediator;
        _snackbar = snackbar;
        _popupService = popupService;
        _store = store;

        _ = GetAllBooksAsync();
    }

    public async Task<BookDetailsDTO?> AddAsync(BookCommandDTO item)
    {
        try
        {
            var book = await _mediator.Send(new AddBook.Command(item));
            _store.Insert(book);
            _snackbar.Add("本を追加しました。", Severity.Info);
            return book;
        }
        catch (EntityValidationException e)
        {
            _snackbar.Add(e.Errors.First().Value.First(), Severity.Error);
            return null;
        }
    }

    public async Task<BookDetailsDTO?> EditAsync(Guid id, BookCommandDTO item)
    {
        try
        {
            var book = await _mediator.Send(new EditBook.Command(item, id));
            _store.Edit(book);
            _snackbar.Add("本を保存しました。", Severity.Info);
            return book;
        }
        catch (EntityValidationException e)
        {
            _snackbar.Add(e.Errors.First().Value.First(), Severity.Error);
            return null;
        }
    }

    public async Task DeleteAsync(BookDetailsDTO item)
    {
        bool confirm = await _popupService.ShowConfirm("確認", "この本を削除しますか？");
        if (!confirm) return;

        await _mediator.Send(new DeleteBook.Command(item.Id));
        _store.Remove(item);
        _snackbar.Add("本を削除しました。", Severity.Info);
    }

    public async Task GetAllBooksAsync()
    {
        var items = await _mediator.Send(new GetBookList.Query());
        _store.Set(items);
    }
}
