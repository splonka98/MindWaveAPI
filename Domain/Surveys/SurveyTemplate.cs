namespace Domain.Surveys;

public sealed class SurveyTemplate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty; // e.g., "daily"
    public string EpisodePath { get; private set; } = string.Empty; // "initial" | "depression" | "hypomania" | "mania"
    public List<SurveyQuestion> Questions { get; private set; } = new();

    private SurveyTemplate() { }

    public static SurveyTemplate Create(Guid id, string name, string episodePath)
    {
        return new SurveyTemplate { Id = id, Name = name, EpisodePath = episodePath };
    }

    public void AddQuestion(int id, string text, int order)
    {
        Questions.Add(SurveyQuestion.Create(id, text, order));
    }
}