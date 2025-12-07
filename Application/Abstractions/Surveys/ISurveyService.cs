using Application.Contracts.Surveys;

namespace Application.Abstractions.Surveys;

public interface ISurveyService
{
    Task<SubmitInitialAnswersResponse> SubmitInitialAnswersAsync(SubmitInitialAnswersRequest request, CancellationToken ct);
    Task<SubmitFollowupAnswersResponse> SubmitFollowupAnswersAsync(SubmitFollowupAnswersRequest request, CancellationToken ct);
}