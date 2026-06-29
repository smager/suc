using Microsoft.AspNetCore.DataProtection;

namespace SmagerUp.Core.API.Services;
public class EncryptionService : IEncryptionService
{
    private readonly IDataProtector _protector;

    public EncryptionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("SmagerUp.Core");
    }

    public string Encrypt(string text)
    {
        return _protector.Protect(text);
    }

    public string Decrypt(string cipher)
    {
        return _protector.Unprotect(cipher);
    }
}
