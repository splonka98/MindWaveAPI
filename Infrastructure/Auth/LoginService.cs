using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public sealed class LoginService : ILoginService
{
    private readonly MindWaveDbContext _db;
    private readonly IConfiguration _config;

    public LoginService(MindWaveDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<Result> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return new Failure(ErrorCodes.Validation, "Request body is required.");
            }

            // Używamy Email zamiast Username
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new Failure(ErrorCodes.Validation, "Email and password are required.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user is null)
            {
                return new Failure(ErrorCodes.Unauthorized, "Invalid credentials.");
            }

            // Prosta walidacja hasła (dopasowana do RegistrationService, które zapisuje plaintext)
            // Jeśli w projekcie jest hashing, zastąp to odpowiednim sprawdzeniem.
            if (!user.VerifyPassword(request.Password))
            {
                return new Failure(ErrorCodes.Unauthorized, "Invalid credentials.");
            }

            // Read and validate JWT settings
            var jwtSection = _config.GetSection("Jwt");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var key = jwtSection["Key"];
            var lifetimeStr = jwtSection["AccessTokenLifetimeMinutes"];

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                string.IsNullOrWhiteSpace(key))
            {
                return new Failure(ErrorCodes.Validation, "JWT configuration is missing required values (Issuer, Audience, Key).");
            }

            if (!int.TryParse(lifetimeStr, out var lifetimeMinutes) || lifetimeMinutes <= 0)
            {
                lifetimeMinutes = 60;
            }

            // Validate HMAC-SHA256 key strength to avoid ArgumentOutOfRangeException
            byte[] keyBytes;
            if (string.IsNullOrWhiteSpace(key))
            {
                return new Failure(ErrorCodes.Validation, "JWT configuration is missing required values (Issuer, Audience, Key).");
            }

            // If the key is Base64, decode it; otherwise use UTF8 bytes.
            // Adjust as needed based on how your key is stored.
            try
            {
                keyBytes = Convert.TryFromBase64String(key, new Span<byte>(new byte[key.Length]), out var _)
                    ? Convert.FromBase64String(key)
                    : Encoding.UTF8.GetBytes(key);
            }
            catch
            {
                keyBytes = Encoding.UTF8.GetBytes(key);
            }

            // Enforce minimum key length for HS256 (recommend >= 32 bytes)
            if (keyBytes.Length < 32)
            {
                return new Failure(ErrorCodes.Validation, "JWT signing key is too short. Use a key of at least 32 bytes.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new Success<LoginResponse>(new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = tokenString
            });
        }
        catch (OperationCanceledException)
        {
            return new Failure(ErrorCodes.Validation, "Login operation was canceled.");
        }
        catch
        {
            // Zwracamy generyczny błąd bez wycieku szczegółów
            return new Failure(ErrorCodes.Unknown, "An unexpected error occurred during login.");
        }
    }
}