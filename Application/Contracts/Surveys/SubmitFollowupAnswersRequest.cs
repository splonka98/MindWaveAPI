namespace Application.Contracts.Surveys;

public sealed class SubmitFollowupAnswersRequest
{
    public Guid SurveyInstanceId { get; init; }
    public string NextQuestionSet { get; init; } = string.Empty; // "mania" or "depression" (echo from initial response)
    public Dictionary<int, string> Answers { get; init; } = new(); // 7 answers mapped by questionId
}