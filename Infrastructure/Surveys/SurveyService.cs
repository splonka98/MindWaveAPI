using Application.Abstractions.Surveys;
using Application.Contracts.Surveys;
using Domain.Surveys;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Surveys;

public sealed class SurveyService : ISurveyService
{
    private readonly MindWaveDbContext _db;

    public SurveyService(MindWaveDbContext db) => _db = db;

    public async Task<SubmitInitialAnswersResponse> SubmitInitialAnswersAsync(SubmitInitialAnswersRequest request, CancellationToken ct)
    {
        // Ensure only one survey per day
        var existing = await _db.SurveyInstances.FirstOrDefaultAsync(
            s => s.PatientUserId == request.PatientUserId && s.Date == request.Date, ct);

        if (existing is null)
        {
            existing = SurveyInstance.Create(Guid.NewGuid(), request.PatientUserId, Guid.Empty, request.Date);
            _db.SurveyInstances.Add(existing);
            await _db.SaveChangesAsync(ct);
        }

        // Remove any existing initial answers (1..4)
        await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var initIds = new[] { 1, 2, 3, 4 };
            var toRemove = await _db.SurveyAnswers
                .Where(a => a.SurveyInstanceId == existing.Id && initIds.Contains(a.QuestionId))
                .ToListAsync(ct);
            _db.SurveyAnswers.RemoveRange(toRemove);

            var answers = new[]
            {
                SurveyAnswer.Create(Guid.NewGuid(), existing.Id, request.PatientUserId, request.Date, 1, request.Answer1),
                SurveyAnswer.Create(Guid.NewGuid(), existing.Id, request.PatientUserId, request.Date, 2, request.Answer2),
                SurveyAnswer.Create(Guid.NewGuid(), existing.Id, request.PatientUserId, request.Date, 3, request.Answer3),
                SurveyAnswer.Create(Guid.NewGuid(), existing.Id, request.PatientUserId, request.Date, 4, request.Answer4),
            };
            _db.SurveyAnswers.AddRange(answers);
            await _db.SaveChangesAsync(ct);
            await _db.Database.CommitTransactionAsync(ct);
        }
        catch
        {
            await _db.Database.RollbackTransactionAsync(ct);
            throw;
        }

        var category = DetermineCategory(request.Answer1, request.Answer2, request.Answer3, request.Answer4);
        var nextIds = category switch
        {
            "depression" => DepressionQuestionIds(),
            "hypomania" => HypomaniaQuestionIds(),
            _ => ManiaQuestionIds()
        };

        return new SubmitInitialAnswersResponse
        {
            SurveyInstanceId = existing.Id,
            Category = category,
            NextQuestionIds = nextIds
        };
    }

    public async Task<InitialSurveyCategoryResponse?> GetInitialCategoryAsync(Guid surveyInstanceId, CancellationToken ct)
    {
        var initAnswers = await _db.SurveyAnswers
            .Where(a => a.SurveyInstanceId == surveyInstanceId && a.QuestionId >= 1 && a.QuestionId <= 4)
            .OrderBy(a => a.QuestionId)
            .ToListAsync(ct);

        if (initAnswers.Count != 4)
        {
            return null;
        }

        var sum = initAnswers.Sum(a => a.Value);
        var category = DetermineCategory(sum);

        return new InitialSurveyCategoryResponse
        {
            SurveyInstanceId = surveyInstanceId,
            Category = category
        };
    }

    public async Task<SubmitFollowupAnswersResponse> SubmitFollowupAnswersAsync(SubmitFollowupAnswersRequest request, CancellationToken ct)
    {
        var instance = await _db.SurveyInstances.FirstOrDefaultAsync(s => s.Id == request.SurveyInstanceId && s.PatientUserId == request.PatientUserId, ct);
        if (instance is null)
        {
            throw new InvalidOperationException("Survey instance not found for patient.");
        }

        // Validate expected question IDs for category
        var expectedIds = request.Category switch
        {
            "depression" => DepressionQuestionIds(),
            "hypomania" => HypomaniaQuestionIds(),
            "mania" => ManiaQuestionIds(),
            _ => throw new InvalidOperationException("Unknown category.")
        };

        if (!expectedIds.SequenceEqual(request.Answers.Keys.OrderBy(k => k)))
        {
            throw new InvalidOperationException("Provided follow-up question IDs do not match expected category.");
        }

        await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // Remove any previous follow-up answers for these IDs to allow resubmission
            var toRemove = await _db.SurveyAnswers
                .Where(a => a.SurveyInstanceId == instance.Id && expectedIds.Contains(a.QuestionId))
                .ToListAsync(ct);
            _db.SurveyAnswers.RemoveRange(toRemove);

            foreach (var kv in request.Answers)
            {
                var ans = SurveyAnswer.Create(Guid.NewGuid(), instance.Id, request.PatientUserId, instance.Date, kv.Key, kv.Value);
                _db.SurveyAnswers.Add(ans);
            }

            var saved = await _db.SaveChangesAsync(ct);
            await _db.Database.CommitTransactionAsync(ct);

            return new SubmitFollowupAnswersResponse
            {
                SurveyInstanceId = instance.Id,
                SavedAnswersCount = saved
            };
        }
        catch
        {
            await _db.Database.RollbackTransactionAsync(ct);
            throw;
        }
    }

    private static string DetermineCategory(int a1, int a2, int a3, int a4)
    {
        var sum = a1 + a2 + a3 + a4;
        return DetermineCategory(sum);
    }

    private static string DetermineCategory(int sum)
    {
        if (sum < 16) return "depression";
        if (sum >= 16 && sum < 32) return "hypomania";
        return "mania";
    }

    private static int[] DepressionQuestionIds() => new[] { 101, 102, 103, 104, 105, 106, 107 };
    private static int[] HypomaniaQuestionIds() => new[] { 201, 202, 203, 204, 205 };
    private static int[] ManiaQuestionIds() => new[] { 301, 302, 303, 304, 305, 306 };
}