using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth;

public sealed class RegistrationService : IRegistrationService
{
    private readonly MindWaveDbContext _db;

    public RegistrationService(MindWaveDbContext db) => _db = db;

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return new Failure(ErrorCodes.Validation, "Email and password are required.");

        if (request.Role is not ("Patient" or "Doctor"))
            return new Failure(ErrorCodes.Validation, "Role must be 'Patient' or 'Doctor'.");

        var exists = await _db.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (exists)
            return new Failure(ErrorCodes.Validation, "Email already registered.");

        var user = User.Create(Guid.NewGuid(), request.Email, request.Password, request.Role, DateTime.UtcNow);
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return new Success<RegisterResponse>(new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role
        });
    }
}