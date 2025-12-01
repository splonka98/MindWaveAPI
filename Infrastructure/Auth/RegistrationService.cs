using Application.Abstractions.Auth;
using Application.Contracts.Auth;
using Application.Contracts.Common;

namespace Infrastructure.Auth;

public sealed class RegistrationService : IRegistrationService
{
    public Task<Result> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult<Result>(new Failure(ErrorCodes.Validation, "Email and password are required."));
        }

        if (request.Role is not ("Patient" or "Doctor"))
        {
            return Task.FromResult<Result>(new Failure(ErrorCodes.Validation, "Role must be 'Patient' or 'Doctor'."));
        }

        // Persist user, hash password, enforce unique email, etc.
        var userId = Guid.NewGuid();
        var response = new RegisterResponse
        {
            UserId = userId,
            Email = request.Email,
            Role = request.Role
        };
        return Task.FromResult<Result>(new Success<RegisterResponse>(response));
    }
}