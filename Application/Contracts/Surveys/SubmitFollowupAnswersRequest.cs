using System.Collections.Generic;

namespace Application.Contracts.Surveys;

public sealed class SubmitFollowupAnswersRequest
{
    public Guid SurveyInstanceId { get; init; }
    public Guid PatientUserId { get; init; }
    public string Category { get; init; } = string.Empty; // "depression" | "hypomania" | "mania"
    // Key = QuestionId, Value = numeric 1..10 string payload allowed in future, here int
    public Dictionary<int, int> Answers { get; init; } = new();
}