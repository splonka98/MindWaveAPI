namespace Application.Contracts.Surveys;

public sealed class SubmitInitialAnswersResponse
{
    public Guid SurveyInstanceId { get; init; }
    public string Category { get; init; } = string.Empty; // "depression" | "hypomania" | "mania"
    public int[] NextQuestionIds { get; init; } = Array.Empty<int>();
}