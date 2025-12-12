using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Surveys;
using Application.Contracts.Common;
using Application.Contracts.Surveys;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MindWaveAPI.Controllers;
using Moq;
using Xunit;
using System.Security.Claims;

namespace MindWaveApi.Tests.Controllers;

public sealed class DailySurveyControllerTests
{
    [Fact]
    public async Task SubmitInitial_Returns200_OnSuccess()
    {
        var service = new Mock<ISurveyService>();
        var dto = new SubmitInitialAnswersResponse { Category = "A" };
        service.Setup(s => s.SubmitInitialAnswersAsync(It.IsAny<SubmitInitialAnswersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var sut = new DailySurveyController(service.Object);

        var userId = Guid.NewGuid();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", userId.ToString())
        }, "TestAuth"));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        var result = await sut.SubmitInitial(
            new SubmitInitialAnswersRequest
            {
                PatientUserId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Answer1 = 0,
                Answer2 = 0,
                Answer3 = 0,
                Answer4 = 0
            },
            CancellationToken.None
        );

        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().Be(dto);
    }
}