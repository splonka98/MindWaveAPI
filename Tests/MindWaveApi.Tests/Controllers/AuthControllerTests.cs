using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MindWaveAPI.Controllers;
using Moq;
using Xunit;

namespace MindWaveApi.Tests.Controllers;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task Login_Returns200_OnSuccess()
    {
        var loginService = new Mock<ILoginService>();
        var registrationService = new Mock<IRegistrationService>();
        var logger = new Mock<ILogger<AuthController>>();
        var token = new LoginResponse
        {
            UserId = Guid.NewGuid(),
            Email = "e@x.com",
            Role = "User",
            Token = "a"
        };
        loginService.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Success<LoginResponse>(token));

        var sut = new AuthController(loginService.Object, registrationService.Object, logger.Object);

        var result = await sut.Login(new LoginRequest { Email = "e@x.com", Password = "pw" }, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().Be(token);
    }

    [Fact]
    public async Task Login_Returns401_OnUnauthorized()
    {
        var loginService = new Mock<ILoginService>();
        var registrationService = new Mock<IRegistrationService>();
        var logger = new Mock<ILogger<AuthController>>();
        loginService.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Failure(ErrorCodes.Unauthorized, "Invalid"));

        var sut = new AuthController(loginService.Object, registrationService.Object, logger.Object);

        var result = await sut.Login(new LoginRequest { Email = "e@x.com", Password = "bad" }, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}