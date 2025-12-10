namespace Domain.Surveys;

public sealed class SurveyAnswer
{
    public Guid Id { get; private set; }
    public Guid SurveyInstanceId { get; private set; }
    public Guid PatientUserId { get; private set; }
    public DateOnly Date { get; private set; }
    public int QuestionId { get; private set; }
    public int Value { get; private set; }

    private SurveyAnswer() { }

    public static SurveyAnswer Create(Guid id, Guid surveyInstanceId, Guid patientUserId, DateOnly date, int questionId, int value)
    {
        return new SurveyAnswer
        {
            Id = id,
            SurveyInstanceId = surveyInstanceId,
            PatientUserId = patientUserId,
            Date = date,
            QuestionId = questionId,
            Value = value
        };
    }
}