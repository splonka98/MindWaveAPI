namespace Application.Contracts.Surveys;

public sealed class InitialSurveyCategoryResponse
{
    public Guid SurveyInstanceId { get; init; }
    public string Category { get; init; } = string.Empty;
}