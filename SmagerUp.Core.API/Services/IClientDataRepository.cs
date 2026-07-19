using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SmagerUp.Core.API.DTOs;

namespace SmagerUp.Core.API.Data;

public interface IClientDataRepository {
    Task<object> ExecuteAsync(
        string apiKey,
        Guid userId,
        DataRequest request);    

}