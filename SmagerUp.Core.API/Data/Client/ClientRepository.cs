 
namespace SmagerUp.Core.API.Data.Client;

public class ClientRepository
{
    private readonly IClientDbResolver _resolver;

    public ClientRepository(IClientDbResolver resolver) => _resolver = resolver;
}