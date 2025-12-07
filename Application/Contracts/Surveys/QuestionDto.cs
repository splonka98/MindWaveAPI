namespace Application.Contracts.Surveys;

public sealed class QuestionDto
{
    public int Id { get; init; }
    public string Text { get; init; } = string.Empty;
}