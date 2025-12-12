using Domain.Surveys;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Surveys;

public sealed class SurveyInstanceTests
{
    [Fact]
    public void AddAnswer_Adds_Answers()
    {
        var id = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var instance = SurveyInstance.Create(id, patientId, templateId, DateOnly.FromDateTime(DateTime.UtcNow));

        instance.AddAnswer(1, 1);
        instance.AddAnswer(2, 3);

        instance.Answers.Should().HaveCount(2);
    }
}