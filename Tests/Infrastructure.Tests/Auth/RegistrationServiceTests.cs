using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Users;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using FluentAssertions;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests.Auth;

public sealed class RegistrationServiceTests
{
    private static MindWaveDbContext Db()
    {
        var options = new DbContextOptionsBuilder<MindWaveDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MindWaveDbContext(options);
    }

    private static IUserRepository UserRepository(MindWaveDbContext db)
    {
        return new UserRepository(db);
    }

    [Fact]
    public async Task Register_Fails_On_Invalid_Role()
    {
        await using var db = Db();
        var userRepository = UserRepository(db);
        var sut = new RegistrationService(userRepository);

        var res = await sut.RegisterAsync(new RegisterRequest { Email = "a@b.com", Password = "pw", Role = "X" }, CancellationToken.None);

        res.Should().BeOfType<Failure>().Which.Code.Should().Be(ErrorCodes.Validation);
    }
}