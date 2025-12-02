using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth;

public sealed class LoginService : ILoginService
{
    private readonly MindWaveDbContext _db;

    public LoginService(MindWaveDbContext db) => _db = db;

    public async Task<Result> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return new Failure(ErrorCodes.Validation, "Username and password are required.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Username, cancellationToken);
        if (user is null || !user.VerifyPassword(request.Password))
            return new Failure(ErrorCodes.Unauthorized, "Invalid credentials.");

        return new Success<LoginResponse>(new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role
        });
    }
}