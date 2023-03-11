using YomLog.Shared.Models;
using YomLog.BlazorShared.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using YomLog.Shared.DTOs;
using YomLog.Shared.Entities;
using YomLog.Shared.Queries;

namespace YomLog.BlazorShared.Services.Api;

public abstract class ApiServiceBase<TModel>
{
    protected readonly HttpClientWrapper _httpClientWrapper;
    protected virtual string Resource => typeof(TModel).Name.Pluralize();

    protected ApiServiceBase(HttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

}

public abstract class ApiGetServiceBase<TModel, TDetailsDTO> : ApiServiceBase<TModel>, IApiGetService<TModel, TDetailsDTO>
    where TModel : EntityBase<TModel>
    where TDetailsDTO : ModelDetailsDTOBase<TModel, TDetailsDTO>
{
    protected ApiGetServiceBase(HttpClientWrapper httpClientWrapper) : base(httpClientWrapper)
    {
    }

    public virtual Task<TDetailsDTO> Get(Guid id)
        => _httpClientWrapper.Get<TDetailsDTO>($"{Resource}/{id}");
}

public abstract class ApiQueryFilterServiceBase<TModel, TDetailsDTO, TQueryParameter>
    : ApiGetServiceBase<TModel, TDetailsDTO>, IApiQueryFilterService<TModel, TDetailsDTO, TQueryParameter>
    where TModel : EntityBase<TModel>
    where TDetailsDTO : ModelDetailsDTOBase<TModel, TDetailsDTO>
    where TQueryParameter : IQueryParameter<TModel>
{
    protected ApiQueryFilterServiceBase(HttpClientWrapper httpClientWrapper) : base(httpClientWrapper)
    {
    }

    public async Task<Pagination<TDetailsDTO>> GetList(TQueryParameter param)
    {
        var queries = param.AsDictionary();
        var uri = QueryHelpers.AddQueryString(Resource, queries);
        return await _httpClientWrapper.Get<Pagination<TDetailsDTO>>(uri);
    }
}

public abstract class ApiCrudServiceBase<TModel, TDetailsDTO, TCommandDTO, TQueryParameter>
    : ApiQueryFilterServiceBase<TModel, TDetailsDTO, TQueryParameter>, IApiService<TModel, TDetailsDTO, TCommandDTO, TQueryParameter>
    where TModel : EntityBase<TModel>
    where TDetailsDTO : ModelDetailsDTOBase<TModel, TDetailsDTO>
    where TCommandDTO : ICommandDTO<TModel>
    where TQueryParameter : IQueryParameter<TModel>
{
    protected ApiCrudServiceBase(HttpClientWrapper httpClientWrapper) : base(httpClientWrapper)
    {
    }

    public virtual Task<TDetailsDTO> Create(TCommandDTO command)
         => _httpClientWrapper.Create<TDetailsDTO, TCommandDTO>(Resource, command);

    public virtual Task<TDetailsDTO> Put(Guid id, TCommandDTO command)
        => _httpClientWrapper.Put<TDetailsDTO, TCommandDTO>($"{Resource}/{id}", command);

    public virtual Task Delete(Guid id)
         => _httpClientWrapper.Delete($"{Resource}/{id}");
}
