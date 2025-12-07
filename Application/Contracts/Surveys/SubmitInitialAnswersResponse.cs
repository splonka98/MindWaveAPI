namespace Application.Contracts.Surveys;

public sealed class SubmitInitialAnswersResponse
{
    public Guid SurveyInstanceId { get; init; }
    public string NextQuestionSet { get; init; } = string.Empty; // "depression" | "hypomania" | "mania"
    public QuestionDto[] NextQuestions { get; init; } = Array.Empty<QuestionDto>(); // 7 follow-up questions
}