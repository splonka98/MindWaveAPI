using System;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Users;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private User() { }

    public static User Create(Guid id, string email, string rawPassword, string role, DateTime createdAtUtc)
    {
        var user = new User
        {
            Id = id,
            Email = email,
            Role = role,
            CreatedAtUtc = createdAtUtc
        };
        user.PasswordHash = HashPassword(rawPassword);
        return user;
    }

    public bool VerifyPassword(string rawPassword) => Verify(rawPassword, PasswordHash);

    // Simple PBKDF2 (salt:hash format)
    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static bool Verify(string password, string stored)
    {
        var parts = stored.Split(':');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var actual = pbkdf2.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}