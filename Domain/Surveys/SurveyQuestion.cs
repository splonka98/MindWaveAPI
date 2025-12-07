namespace Domain.Surveys;

public sealed class SurveyQuestion
{
    public Guid SurveyTemplateId { get; private set; }
    public int Id { get; private set; } // logical question Id shown to clients
    public string Text { get; private set; } = string.Empty;
    public int Order { get; private set; }

    private SurveyQuestion() { }

    internal static SurveyQuestion Create(int id, string text, int order)
    {
        return new SurveyQuestion { Id = id, Text = text, Order = order };
    }
}