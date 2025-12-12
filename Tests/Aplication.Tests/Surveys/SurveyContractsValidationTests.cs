using Application.Contracts.Surveys;
using FluentAssertions;
using Xunit;

namespace Aplication.Tests.Surveys;

public sealed class SurveyContractsValidationTests
{
    [Fact]
    public void SubmitInitialAnswersRequest_Defaults_Are_Safe()
    {
        var req = new SubmitInitialAnswersRequest
        {
            PatientUserId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        req.PatientUserId.Should().NotBeEmpty();
        req.Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
        req.Answer1.Should().Be(0);
        req.Answer2.Should().Be(0);
        req.Answer3.Should().Be(0);
        req.Answer4.Should().Be(0);
    }
}