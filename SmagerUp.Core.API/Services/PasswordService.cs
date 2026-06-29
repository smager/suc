using Microsoft.AspNetCore.Identity;

namespace SmagerUp.Core.API.Services;

public class PasswordService:IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var result = _hasher.VerifyHashedPassword(
            null!,
            hashedPassword,
            password);

        return result == PasswordVerificationResult.Success;
    }
}
