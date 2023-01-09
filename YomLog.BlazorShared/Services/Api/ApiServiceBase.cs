using YomLog.Shared.Models;
using YomLog.BlazorShared.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using YomLog.Shared.DTOs;
using YomLog.Shared.Entities;
using YomLog.Shared.Queries;

namespace YomLog.BlazorShared.Services.Api;

public abstract class ApiServiceBase<TModel, TResultDTO, TCommandDTO, TQueryParameter>
    : IApiService<TModel, TResultDTO, TCommandDTO, TQueryParameter>
    where TModel : EntityBase<TModel>
    where TResultDTO : ModelResultDTOBase<TModel, TResultDTO>
    where TCommandDTO : ICommandDTO<TModel>
    where TQueryParameter : IQueryParameter<TModel>
{
    protected readonly HttpClientWrapper _httpClientWrapper;

    protected ApiServiceBase(HttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    protected virtual string Resource => $"{typeof(TModel).Name}s";

    public virtual async Task<Pagination<TResultDTO>> GetList(TQueryParameter param)
    {
        var queries = param
            .AsDictionary()
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value!.ToString());
        var uri = QueryHelpers.AddQueryString(Resource, queries);
        return await _httpClientWrapper.Get<Pagination<TResultDTO>>(uri);
    }

    public virtual async Task<TResultDTO> Get(Guid id)
        => await _httpClientWrapper.Get<TResultDTO>($"{Resource}/{id}");

    public virtual async Task<TResultDTO> Create(TCommandDTO command)
         => await _httpClientWrapper.Create<TResultDTO, TCommandDTO>(Resource, command);

    public virtual async Task<TResultDTO> Put(Guid id, TCommandDTO command)
        => await _httpClientWrapper.Put<TResultDTO, TCommandDTO>($"{Resource}/{id}", command);

    public virtual async Task Delete(Guid id)
         => await _httpClientWrapper.Delete($"{Resource}/{id}");
}
