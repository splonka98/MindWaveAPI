namespace Application.Contracts.Surveys;

public sealed class SubmitInitialAnswersRequest
{
    public Guid PatientUserId { get; init; }
    public Guid TemplateId { get; init; } // Daily survey template identifier
    public DateOnly Date { get; init; } // The day these answers belong to
    public string Answer1 { get; init; } = string.Empty; // Q1
    public string Answer2 { get; init; } = string.Empty; // Q2
    public string Answer3 { get; init; } = string.Empty; // Q3
    public string Answer4 { get; init; } = string.Empty; // Q4
}