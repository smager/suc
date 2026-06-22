using SmagerUp.Core.API.DTOs;

namespace SmagerUp.Core.API.Data.Client;

public interface IDataRepository
{
    Task<object> GetDataAsync(
        Guid clientId,
        Guid userId,
        DataRequest request);

    Task<object> ExecuteCmdAsync(
        Guid clientId,
        Guid userId,
        DataRequest request);
}