using SmagerUp.Core.API.DTOs;

namespace SmagerUp.Core.API.Data.Client;

public interface ICoreDataRepository
{
    Task<object> GetDataAsync(
        Guid? userId,
        DataRequest request);

    Task<object> ExecuteCmdAsync(
        Guid? userId,
        DataRequest request);
}