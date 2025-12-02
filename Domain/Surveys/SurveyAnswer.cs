namespace Domain.Surveys;

public sealed class SurveyAnswer
{
    public int QuestionId { get; private set; }
    public string Value { get; private set; } = string.Empty;

    private SurveyAnswer() { }

    public SurveyAnswer(int questionId, string value)
    {
        QuestionId = questionId;
        Value = value;
    }
}