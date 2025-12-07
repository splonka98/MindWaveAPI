namespace Application.Contracts.Surveys;

public sealed class SubmitFollowupAnswersResponse
{
    public Guid SurveyInstanceId { get; init; }
    public int TotalAnswersCount { get; init; } // should be 10 after this step
}