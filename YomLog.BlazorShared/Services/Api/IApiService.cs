using YomLog.Shared.DTOs;
using YomLog.Shared.Entities;
using YomLog.Shared.Models;
using YomLog.Shared.Queries;

namespace YomLog.BlazorShared.Services.Api;

public interface IApiService<TModel, TResultDTO, TCommandDTO, TQueryParameter>
    where TModel : EntityBase<TModel>
    where TResultDTO : ModelResultDTOBase<TModel, TResultDTO>
    where TCommandDTO : ICommandDTO<TModel>
    where TQueryParameter : IQueryParameter<TModel>
{
    Task<Pagination<TResultDTO>> GetList(TQueryParameter param);

    Task<TResultDTO> Get(Guid id);

    Task<TResultDTO> Create(TCommandDTO command);

    Task<TResultDTO> Put(Guid id, TCommandDTO command);

    Task Delete(Guid id);
}
