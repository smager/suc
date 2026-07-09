using System.Data;
namespace SmagerUp.Core.API.Data.Client; 
public interface IClientDbResolver
{
    IDbConnection CreateConnection(Guid ClientId);
}

