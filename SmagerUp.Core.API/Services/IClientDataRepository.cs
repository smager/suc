using SmagerUp.Core.API.DTOs;

namespace SmagerUp.Core.API.Data.Client;

public interface IClientDataRepository {
    Task<object> ExecuteAsync(
        Guid clientId,
        Guid userId,
        DataRequest request);    
}