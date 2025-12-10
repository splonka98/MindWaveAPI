namespace Application.Contracts.Surveys;

public sealed class SubmitInitialAnswersRequest
{
    public Guid PatientUserId { get; init; }
    public DateOnly Date { get; init; }
    public int Answer1 { get; init; } // numeric 1..10
    public int Answer2 { get; init; }
    public int Answer3 { get; init; }
    public int Answer4 { get; init; }
}