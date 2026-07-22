using Microsoft.Extensions.Options;

namespace SmagerUp.Core.API.Services;

public sealed class EncryptionService : IEncryptionService
{
    private readonly Cryptography _crypto;

    public EncryptionService()
    {
        //var opt = options.Value;

        //_crypto = new Cryptography(
        //    opt.PassPhrase,
        //    opt.InitVector);
        _crypto = new Cryptography();
    }

    public string Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return _crypto.Encrypt(text);
    }

    public string Decrypt(string cipher)
    {
        if (string.IsNullOrEmpty(cipher))
            return string.Empty;

        return _crypto.Decrypt(cipher);
    }
}