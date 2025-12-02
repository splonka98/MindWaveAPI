namespace Domain.Surveys;

public sealed class SurveyInstance
{
    public Guid Id { get; private set; }
    public Guid PatientUserId { get; private set; }
    public Guid TemplateId { get; private set; }
    public DateOnly Date { get; private set; }
    public List<SurveyAnswer> Answers { get; private set; } = new();
    public List<string> Tags { get; private set; } = new(); // Ignored in EF mapping currently

    private SurveyInstance() { }

    public static SurveyInstance Create(Guid id, Guid patientUserId, Guid templateId, DateOnly date)
    {
        return new SurveyInstance
        {
            Id = id,
            PatientUserId = patientUserId,
            TemplateId = templateId,
            Date = date
        };
    }

    public void AddAnswer(int questionId, string value)
    {
        Answers.Add(new SurveyAnswer(questionId, value));
    }
}