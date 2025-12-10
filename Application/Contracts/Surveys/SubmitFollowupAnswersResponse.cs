namespace Application.Contracts.Surveys;

public sealed class SubmitFollowupAnswersResponse
{
    public Guid SurveyInstanceId { get; init; }
    public int SavedAnswersCount { get; init; }
}