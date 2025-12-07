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
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<Result> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return new Failure(ErrorCodes.Validation, "Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new Failure(ErrorCodes.Validation, "Username and password are required.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Username, cancellationToken);
            if (user is null || !user.VerifyPassword(request.Password))
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

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
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
        catch (Exception ex)
        {
            // Avoid leaking internal details; return a generic failure
            return new Failure(ErrorCodes.Unknown, "An unexpected error occurred during login.");
        }
    }
}