using YomLog.Shared.DTOs;
using YomLog.Shared.Entities;
using YomLog.Shared.Models;
using YomLog.Shared.Queries;

namespace YomLog.BlazorShared.Services.Api;

public interface IApiService<TModel, TDetailsDTO, TCommandDTO, TQueryParameter>
    : IApiGetService<TModel, TDetailsDTO>,
      IApiQueryFilterService<TModel, TDetailsDTO, TQueryParameter>,
      IApiEditService<TModel, TDetailsDTO, TCommandDTO>,
      IApiDeleteService<TModel>
    where TModel : EntityBase<TModel>
    where TDetailsDTO : ModelDetailsDTOBase<TModel, TDetailsDTO>
    where TCommandDTO : ICommandDTO<TModel>
    where TQueryParameter : IQueryParameter<TModel>
{
}

public interface IApiGetService<TModel, TDetailsDTO>
    where TModel : EntityBase<TModel>
    where TDetailsDTO : ModelDetailsDTOBase<TModel, TDetailsDTO>
{
    Task<TDetailsDTO> Get(Guid id);
}

public interface IApiQueryFilterService<TModel, TDetailsDTO, TQueryParameter>
    : IApiGetService<TModel, TDetailsDTO>
    where TModel : EntityBase<TModel>
    where TDetailsDTO : ModelDetailsDTOBase<TModel, TDetailsDTO>
    where TQueryParameter : IQueryParameter<TModel>
{
    Task<Pagination<TDetailsDTO>> GetList(TQueryParameter param);
}

public interface IApiEditService<TModel, TDetailsDTO, TCommandDTO>
    where TModel : EntityBase<TModel>
    where TDetailsDTO : ModelDetailsDTOBase<TModel, TDetailsDTO>
    where TCommandDTO : ICommandDTO<TModel>
{
    Task<TDetailsDTO> Create(TCommandDTO command);

    Task<TDetailsDTO> Put(Guid id, TCommandDTO command);
}

public interface IApiDeleteService<TModel> where TModel : EntityBase<TModel>
{
    Task Delete(Guid id);
}