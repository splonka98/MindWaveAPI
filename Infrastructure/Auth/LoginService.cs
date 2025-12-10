using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Application.Abstractions.Users;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public sealed class LoginService : ILoginService
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _users;

    public LoginService(IConfiguration config, IUserRepository users)
    {
        _config = config;
        _users = users;
    }

    public async Task<Result> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return new Failure(ErrorCodes.Validation, "Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new Failure(ErrorCodes.Validation, "Email and password are required.");
            }

            var user = await _users.FindByEmailAsync(request.Email, cancellationToken);
            if (user is null)
            {
                return new Failure(ErrorCodes.Unauthorized, "Invalid credentials.");
            }

            if (!user.VerifyPassword(request.Password))
            {
                return new Failure(ErrorCodes.Unauthorized, "Invalid credentials.");
            }

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

            byte[] keyBytes;
            try
            {
                // If Base64, decode; otherwise use the raw UTF-8 bytes
                try
                {
                    keyBytes = Convert.FromBase64String(key);
                }
                catch (FormatException)
                {
                    keyBytes = Encoding.UTF8.GetBytes(key);
                }
            }
            catch
            {
                keyBytes = Encoding.UTF8.GetBytes(key);
            }

            if (keyBytes.Length < 32)
            {
                return new Failure(ErrorCodes.Validation, "JWT signing key is too short. Use a key of at least 32 bytes.");
            }

            var claims = new List<Claim>
            {
                new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Id.ToString()),
                new Claim(type:JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                claims.Add(new Claim(type: ClaimTypes.Role, value: user.Role));
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