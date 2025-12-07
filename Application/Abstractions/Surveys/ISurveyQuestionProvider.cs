using Application.Contracts.Surveys;

namespace Application.Abstractions.Surveys;

public interface ISurveyQuestionProvider
{
    // First 4 initiating questions
    QuestionDto[] GetInitiatingQuestions();

    // Follow-up 7 questions by path
    QuestionDto[] GetFollowupQuestions(string path);

    // Get IDs only (for validation)
    int[] GetFollowupQuestionIds(string path);
}