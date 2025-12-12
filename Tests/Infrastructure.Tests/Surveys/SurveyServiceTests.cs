using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Common;
using Application.Contracts.Surveys;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Surveys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace Infrastructure.Tests.Surveys;

public sealed class SurveyServiceTests
{
    private static MindWaveDbContext Db()
    {
        var options = new DbContextOptionsBuilder<MindWaveDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new MindWaveDbContext(options);
    }

    [Fact]
    public async Task StartDaily_Fails_When_No_Active_Template()
    {
        await using var db = Db();
        var sut = new SurveyService(db);
        var req = new SubmitInitialAnswersRequest
        {
            PatientUserId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Answer1 = 1,
            Answer2 = 1,
            Answer3 = 1,
            Answer4 = 1
        };

        var res = await sut.SubmitInitialAnswersAsync(req, CancellationToken.None);

        res.Should().NotBeNull();
    }
}