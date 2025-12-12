using System;
using System.Collections.Generic;

namespace Application.Contracts.Doctors;

public sealed class PatientSurveyWithAnswersDto
{
    public Guid SurveyInstanceId { get; init; }
    public DateOnly Date { get; init; }
    public string Category { get; init; } = string.Empty;
    public IReadOnlyList<SurveyAnswerDto> Answers { get; init; } = Array.Empty<SurveyAnswerDto>();

    public sealed class SurveyAnswerDto
    {
        public int QuestionId { get; init; }
        public int Value { get; init; }
    }
}