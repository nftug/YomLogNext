using YomLog.Shared.Models;
using YomLog.BlazorShared.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using YomLog.Shared.DTOs;
using YomLog.Shared.Entities;
using YomLog.Shared.Queries;

namespace YomLog.BlazorShared.Services.Api;

public abstract class ApiServiceBase<TEntity>
{
    protected readonly HttpClientWrapper _httpClientWrapper;
    protected virtual string Resource => typeof(TEntity).Name.Pluralize();

    protected ApiServiceBase(HttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

}

public abstract class ApiGetServiceBase<TEntity, TDetailsDTO> : ApiServiceBase<TEntity>, IApiGetService<TEntity, TDetailsDTO>
    where TEntity : EntityBase<TEntity>
    where TDetailsDTO : EntityDetailsDTOBase<TEntity, TDetailsDTO>
{
    protected ApiGetServiceBase(HttpClientWrapper httpClientWrapper) : base(httpClientWrapper)
    {
    }

    public virtual Task<TDetailsDTO> Get(Guid id)
        => _httpClientWrapper.Get<TDetailsDTO>($"{Resource}/{id}");
}

public abstract class ApiQueryFilterServiceBase<TEntity, TDetailsDTO, TQueryParameter>
    : ApiGetServiceBase<TEntity, TDetailsDTO>, IApiQueryFilterService<TEntity, TDetailsDTO, TQueryParameter>
    where TEntity : EntityBase<TEntity>
    where TDetailsDTO : EntityDetailsDTOBase<TEntity, TDetailsDTO>
    where TQueryParameter : IQueryParameter<TEntity>
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

public abstract class ApiCrudServiceBase<TEntity, TDetailsDTO, TCommandDTO, TQueryParameter>
    : ApiQueryFilterServiceBase<TEntity, TDetailsDTO, TQueryParameter>, IApiService<TEntity, TDetailsDTO, TCommandDTO, TQueryParameter>
    where TEntity : EntityBase<TEntity>
    where TDetailsDTO : EntityDetailsDTOBase<TEntity, TDetailsDTO>
    where TCommandDTO : ICommandDTO<TEntity>
    where TQueryParameter : IQueryParameter<TEntity>
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
