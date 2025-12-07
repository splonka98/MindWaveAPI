using Application.Abstractions.Surveys;
using Application.Contracts.Surveys;
using Domain.Surveys;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Surveys;

public sealed class SurveyService : ISurveyService
{
    private readonly MindWaveDbContext _db;

    public SurveyService(MindWaveDbContext db)
    {
        _db = db;
    }

    public async Task<SubmitInitialAnswersResponse> SubmitInitialAnswersAsync(SubmitInitialAnswersRequest request, CancellationToken ct)
    {
        // Ensure single survey per patient per date
        var existing = await _db.SurveyInstances
            .FirstOrDefaultAsync(s => s.PatientUserId == request.PatientUserId && s.Date == request.Date, ct);

        if (existing is not null)
        {
            // Allow updating the first four answers
            existing.Answers.RemoveAll(a => a.QuestionId is >= 1 and <= 4);
            existing.AddAnswer(1, request.Answer1);
            existing.AddAnswer(2, request.Answer2);
            existing.AddAnswer(3, request.Answer3);
            existing.AddAnswer(4, request.Answer4);

            await _db.SaveChangesAsync(ct);

            var path = DeterminePath(request.Answer1, request.Answer2, request.Answer3, request.Answer4);
            var nextIds = path == "mania" ? ManiaQuestionIds() : DepressionQuestionIds();

            return new SubmitInitialAnswersResponse
            {
                SurveyInstanceId = existing.Id,
                NextQuestionSet = path,
                NextQuestions = nextIds.Select(id => new QuestionDto { Id = id }).ToArray()
            };
        }

        var instance = SurveyInstance.Create(Guid.NewGuid(), request.PatientUserId, request.TemplateId, request.Date);
        instance.AddAnswer(1, request.Answer1);
        instance.AddAnswer(2, request.Answer2);
        instance.AddAnswer(3, request.Answer3);
        instance.AddAnswer(4, request.Answer4);

        _db.SurveyInstances.Add(instance);
        await _db.SaveChangesAsync(ct);

        var nextPath = DeterminePath(request.Answer1, request.Answer2, request.Answer3, request.Answer4);
        var questions = nextPath == "mania" ? ManiaQuestionIds() : DepressionQuestionIds();

        return new SubmitInitialAnswersResponse
        {
            SurveyInstanceId = instance.Id,
            NextQuestionSet = nextPath,
            NextQuestions = questions.Select(id => new QuestionDto { Id = id }).ToArray()
        };
    }

    public async Task<SubmitFollowupAnswersResponse> SubmitFollowupAnswersAsync(SubmitFollowupAnswersRequest request, CancellationToken ct)
    {
        var instance = await _db.SurveyInstances
            .FirstOrDefaultAsync(s => s.Id == request.SurveyInstanceId, ct);

        if (instance is null)
        {
            throw new InvalidOperationException("Survey instance not found.");
        }

        // Validate question set consistency for 7 follow-up answers
        var expectedIds = request.NextQuestionSet == "mania" ? ManiaQuestionIds() : DepressionQuestionIds();
        var providedIds = request.Answers.Keys.OrderBy(k => k).ToArray();
        if (!expectedIds.SequenceEqual(providedIds))
        {
            throw new InvalidOperationException("Provided follow-up question IDs do not match expected set.");
        }

        // Remove any existing follow-up answers to allow re-submit
        foreach (var qId in expectedIds)
        {
            instance.Answers.RemoveAll(a => a.QuestionId == qId);
        }

        foreach (var kvp in request.Answers)
        {
            instance.AddAnswer(kvp.Key, kvp.Value);
        }

        await _db.SaveChangesAsync(ct);

        return new SubmitFollowupAnswersResponse
        {
            SurveyInstanceId = instance.Id,
            TotalAnswersCount = instance.Answers.Count
        };
    }

    private static string DeterminePath(string a1, string a2, string a3, string a4)
    {
        // Placeholder heuristic using 4 initiating answers:
        // Treat numeric-like inputs "0..5". Non-numeric = 0.
        int v1 = ParseInt(a1);
        int v2 = ParseInt(a2);
        int v3 = ParseInt(a3);
        int v4 = ParseInt(a4);
        var sum = v1 + v2 + v3 + v4;
        // Tune threshold to clinical rule as needed
        return sum >= 12 ? "mania" : "depression";
    }

    private static int ParseInt(string s) => int.TryParse(s, out var v) ? v : 0;

    // After 4 initial questions, we need 7 follow-up questions to reach total 11.
    private static int[] ManiaQuestionIds() => new[] { 5, 6, 7, 8, 9, 10, 11 };
    private static int[] DepressionQuestionIds() => new[] { 105, 106, 107, 108, 109, 110, 111 };
}