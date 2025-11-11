namespace eMechanic.Infrastructure.Identity;

using System.Security.Cryptography;

public class RefreshToken
{
    private RefreshToken(string token, Guid jti, DateTime expiryDate, Guid identityId)
    {
        Id = Guid.NewGuid();
        Token = token;
        Jti = jti;
        CreatedAt = DateTime.UtcNow;
        ExpiryDate = expiryDate;
        IdentityId = identityId;
    }

    private RefreshToken() {}

    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public Guid Jti { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime? InvalidatedAt { get; private set; }
    public Guid IdentityId { get; private set; }

    public bool IsActive => UsedAt == null && InvalidatedAt == null && ExpiryDate > DateTime.UtcNow;

    public static RefreshToken Create(Guid jit, DateTime expiryDate, Guid identityId)
    {
        var tokenValue = GenerateTokenValue();
        return new RefreshToken(tokenValue, jit, expiryDate, identityId);
    }

    public void SetUsed() => UsedAt = DateTime.UtcNow;

    public void SetInvalidated() => InvalidatedAt = DateTime.UtcNow;

    private static string GenerateTokenValue()
    {
        var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = new char[32];
        var randomBytes = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        for (var i = 0; i < 32; i++)
        {
            result[i] = chars[randomBytes[i] % chars.Length];
        }

        return new string(result);
    }
}
