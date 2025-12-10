namespace Application.Contracts.Doctors;

public sealed class PatientSurveyDto
{
    public Guid SurveyInstanceId { get; init; }
    public DateOnly Date { get; init; }
    public string Category { get; init; } = string.Empty;
}