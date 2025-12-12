using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using MindWaveAPI.Controllers;
using Application.Abstractions.Doctors;
using Application.Contracts.Doctors;

namespace MindWaveApi.Tests.Controllers;

public sealed class DoctorControllerTests
{
    [Fact]
    public async Task Pair_ReturnsNoContent_OnSuccess()
    {
        var svc = new Mock<IDoctorPatientService>();
        svc.Setup(s => s.PairAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var sut = new DoctorController(svc.Object);

        // provide authenticated doctor id
        var doctorId = Guid.NewGuid().ToString();
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, doctorId) };
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };

        var req = new PairPatientRequest { PatientUserId = Guid.NewGuid() };

        var result = await sut.PairPatient(req, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }
}