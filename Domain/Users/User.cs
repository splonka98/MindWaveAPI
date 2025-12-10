using System;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Users;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;
    public int PasswordIterations { get; private set; } = 0;
    public string Role { get; private set; } = "Patient";

    private User() { }

    public static User Create(Guid id, string email, string role)
    {
        return new User
        {
            Id = id,
            Email = email,
            Role = role
        };
    }

    public void SetPassword(string plainText, int iterations = 100_000)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(plainText));
        }

        // Generate 16-byte salt
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var hashBytes = HashPassword(plainText, saltBytes, iterations);
        PasswordSalt = Convert.ToBase64String(saltBytes);
        PasswordHash = Convert.ToBase64String(hashBytes);
        PasswordIterations = iterations;
    }

    public bool VerifyPassword(string plainText)
    {
        if (string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(PasswordSalt) || PasswordIterations <= 0)
        {
            return false;
        }

        var saltBytes = Convert.FromBase64String(PasswordSalt);
        var expectedHash = Convert.FromBase64String(PasswordHash);
        var actualHash = HashPassword(plainText, saltBytes, PasswordIterations);

        return FixedTimeEquals(expectedHash, actualHash);
    }

    private static byte[] HashPassword(string password, byte[] salt, int iterations)
    {
        // PBKDF2 with HMAC-SHA256, 32-byte output
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32);
    }

    private static bool FixedTimeEquals(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        if (a.Length != b.Length) return false;
        int diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }
        return diff == 0;
    }
}