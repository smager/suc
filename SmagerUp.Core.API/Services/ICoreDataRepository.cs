using SmagerUp.Core.API.DTOs;

namespace SmagerUp.Core.API.Data.Client;

public interface ICoreDataRepository
{
    Task<object> ExecuteAsync(
        Guid userId,
        DataRequest request);

  
}