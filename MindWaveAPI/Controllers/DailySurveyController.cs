using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Abstractions.Surveys;
using Application.Contracts.Surveys;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MindWaveAPI.Controllers;

[ApiController]
[Route("api/surveys/daily")]
public sealed class DailySurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public DailySurveyController(ISurveyService surveyService) => _surveyService = surveyService;

    [HttpPost("initial")]
    [Authorize]
    public async Task<IActionResult> SubmitInitial([FromBody] SubmitInitialAnswersRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var tokenUserId))
        {
            return Unauthorized("Missing or invalid 'sub' claim.");
        }

        var fixedRequest = new SubmitInitialAnswersRequest
        {
            PatientUserId = tokenUserId,
            Date = request.Date,
            Answer1 = request.Answer1,
            Answer2 = request.Answer2,
            Answer3 = request.Answer3,
            Answer4 = request.Answer4
        };

        var resp = await _surveyService.SubmitInitialAnswersAsync(fixedRequest, ct);
        return Ok(resp);
    }

    [HttpGet("{surveyInstanceId:guid}/category")]
    [Authorize]
    public async Task<IActionResult> GetCategory([FromRoute] Guid surveyInstanceId, CancellationToken ct)
    {
        var resp = await _surveyService.GetInitialCategoryAsync(surveyInstanceId, ct);
        if (resp is null) return NotFound();
        return Ok(resp);
    }

    [HttpPost("followup")]
    [Authorize]
    public async Task<IActionResult> SubmitFollowup([FromBody] SubmitFollowupAnswersRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var tokenUserId))
        {
            return Unauthorized("Missing or invalid 'sub' claim.");
        }

        // Trust the token, not the body
        var fixedRequest = new SubmitFollowupAnswersRequest
        {
            SurveyInstanceId = request.SurveyInstanceId,
            PatientUserId = tokenUserId,
            Category = request.Category,
            Answers = request.Answers
        };

        var resp = await _surveyService.SubmitFollowupAnswersAsync(fixedRequest, ct);
        return Ok(resp);
    }
}