using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Application.Abstractions.Users;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using Domain.Users;

namespace Infrastructure.Auth;

public sealed class RegistrationService : IRegistrationService
{
    private readonly IUserRepository _users;

    public RegistrationService(IUserRepository users) => _users = users;

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new Failure(ErrorCodes.Validation, "Email and password are required.");
        }

        var existing = await _users.FindByEmailAsync(request.Email, ct);
        if (existing is not null)
        {
            return new Failure(ErrorCodes.Validation, "Email already registered.");
        }

        var roleInput = string.IsNullOrWhiteSpace(request.Role) ? "Patient" : request.Role;

        // Allowed roles and normalization
        var allowedRoles = new[] { "Patient", "Doctor", "Admin" };
        var normalizedRole = allowedRoles.FirstOrDefault(r => string.Equals(r, roleInput, StringComparison.OrdinalIgnoreCase));
        if (normalizedRole is null)
        {
            return new Failure(ErrorCodes.Validation, "Invalid role.");
        }

        var user = User.Create(Guid.NewGuid(), request.Email, normalizedRole);
        user.SetPassword(request.Password);

        var saved = await _users.AddAsync(user, ct);
        if (!saved)
        {
            return new Failure(ErrorCodes.Unknown, "Failed to create user.");
        }

        return new Success<RegisterResponse>(new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role
        });
    }
}