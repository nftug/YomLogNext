using YomLog.Shared.DTOs;
using YomLog.Shared.Entities;
using YomLog.Shared.Models;
using YomLog.Shared.Queries;

namespace YomLog.BlazorShared.Services.Api;

public interface IApiService<TEntity, TDetailsDTO, TCommandDTO, TQueryParameter>
    : IApiGetService<TEntity, TDetailsDTO>,
      IApiQueryFilterService<TEntity, TDetailsDTO, TQueryParameter>,
      IApiEditService<TEntity, TDetailsDTO, TCommandDTO>,
      IApiDeleteService<TEntity>
    where TEntity : EntityBase<TEntity>
    where TDetailsDTO : EntityDetailsDTOBase<TEntity, TDetailsDTO>
    where TCommandDTO : ICommandDTO<TEntity>
    where TQueryParameter : IQueryParameter<TEntity>
{
}

public interface IApiGetService<TEntity, TDetailsDTO>
    where TEntity : EntityBase<TEntity>
    where TDetailsDTO : EntityDetailsDTOBase<TEntity, TDetailsDTO>
{
    Task<TDetailsDTO> Get(Guid id);
}

public interface IApiQueryFilterService<TEntity, TDetailsDTO, TQueryParameter>
    : IApiGetService<TEntity, TDetailsDTO>
    where TEntity : EntityBase<TEntity>
    where TDetailsDTO : EntityDetailsDTOBase<TEntity, TDetailsDTO>
    where TQueryParameter : IQueryParameter<TEntity>
{
    Task<Pagination<TDetailsDTO>> GetList(TQueryParameter param);
}

public interface IApiEditService<TEntity, TDetailsDTO, TCommandDTO>
    where TEntity : EntityBase<TEntity>
    where TDetailsDTO : EntityDetailsDTOBase<TEntity, TDetailsDTO>
    where TCommandDTO : ICommandDTO<TEntity>
{
    Task<TDetailsDTO> Create(TCommandDTO command);

    Task<TDetailsDTO> Put(Guid id, TCommandDTO command);
}

public interface IApiDeleteService<TEntity> where TEntity : EntityBase<TEntity>
{
    Task Delete(Guid id);
}