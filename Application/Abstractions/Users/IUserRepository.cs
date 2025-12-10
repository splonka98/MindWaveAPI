using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Users;

namespace Application.Abstractions.Users;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct);
    Task<bool> AddAsync(User user, CancellationToken ct);
}