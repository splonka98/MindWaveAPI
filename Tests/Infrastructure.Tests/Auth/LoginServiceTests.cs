using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Users;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using FluentAssertions;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Auth;

public sealed class LoginServiceTests
{
    private static MindWaveDbContext Db()
    {
        var options = new DbContextOptionsBuilder<MindWaveDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MindWaveDbContext(options);
    }

    [Fact]
    public async Task Login_Fails_When_User_Not_Found()
    {
        await using var db = Db();

        var configMock = new Mock<IConfiguration>();
        var userRepoMock = new Mock<IUserRepository>();

        var sut = new LoginService(configMock.Object, userRepoMock.Object);

        var result = await sut.LoginAsync(new LoginRequest { Email = "x@x.com", Password = "pw" }, CancellationToken.None);

        result.Should().BeOfType<Failure>().Which.Code.Should().Be(ErrorCodes.Unauthorized);
    }
}