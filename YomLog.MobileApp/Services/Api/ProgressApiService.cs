using MediatR;
using MudBlazor;
using YomLog.BlazorShared.Services.Popup;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.Shared.Attributes;
using YomLog.Shared.Exceptions;
using YomLog.UseCase.Books;

namespace YomLog.MobileApp.Services.Api;

[InjectAsScoped]
public class ProgressApiService
{
    protected readonly ISender _mediator;
    protected readonly ISnackbar _snackbar;
    protected readonly IPopupService _popupService;

    public ProgressApiService(ISender mediator, ISnackbar snackbar, IPopupService popupService)
    {
        _mediator = mediator;
        _snackbar = snackbar;
        _popupService = popupService;
    }

    public Task<List<ProgressDetailsDTO>> FetchByBookAsync(Guid bookId)
        => _mediator.Send(new GetProgressList.Query(bookId));

    public async Task<ProgressDetailsDTO?> AddAsync(ProgressCommandDTO item)
    {
        try
        {
            var prog = await _mediator.Send(new AddProgress.Command(item));
            _snackbar.Add("進捗状況を更新しました。", Severity.Info);
            return prog;
        }
        catch (EntityValidationException e)
        {
            _snackbar.Add(e.Errors.First().Value.First(), Severity.Error);
            return null;
        }
    }

    public async Task<ProgressDetailsDTO?> EditAsync(Guid id, ProgressCommandDTO item)
    {
        try
        {
            var prog = await _mediator.Send(new EditProgress.Command(item, id));
            _snackbar.Add("進捗状況を更新しました。", Severity.Info);
            return prog;
        }
        catch (EntityValidationException e)
        {
            _snackbar.Add(e.Errors.First().Value.First(), Severity.Error);
            return null;
        }
    }

    public async Task DeleteAsync(ProgressDetailsDTO item)
    {
        bool confirm = await _popupService.ShowConfirm("確認", "この進捗記録を削除しますか？");
        if (!confirm) return;

        await _mediator.Send(new DeleteProgress.Command(item.Id));
        _snackbar.Add("進捗記録を削除しました。", Severity.Info);
    }
}
