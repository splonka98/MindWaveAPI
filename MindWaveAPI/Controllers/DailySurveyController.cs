using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Abstractions.Surveys;
using Application.Contracts.Surveys;

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
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;
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
        var userIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var tokenUserId))
        {
            return Unauthorized("Missing or invalid 'sub' claim.");
        }

        // 1) Odczytaj kategorię z initial ankiety i porównaj z żądaną
        var initialCategory = await _surveyService.GetInitialCategoryAsync(request.SurveyInstanceId, ct);
        if (initialCategory is null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid survey instance",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Initial survey not found or incomplete."
            });
        }

        if (!string.Equals(initialCategory.Category, request.Category, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Category mismatch",
                Status = StatusCodes.Status400BadRequest,
                Detail = $"Follow-up category '{request.Category}' does not match initial category '{initialCategory.Category}'."
            });
        }

        // 2) Zaufaj tokenowi zamiast patientUserId z body
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