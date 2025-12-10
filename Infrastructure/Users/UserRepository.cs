using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Users;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public sealed class UserRepository : IUserRepository
{
    private readonly MindWaveDbContext _db;

    public UserRepository(MindWaveDbContext db) => _db = db;

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct)
    {
        return _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<bool> AddAsync(User user, CancellationToken ct)
    {
        await _db.Users.AddAsync(user, ct);
        return await _db.SaveChangesAsync(ct) > 0;
    }
}