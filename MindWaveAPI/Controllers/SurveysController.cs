using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Abstractions.Surveys;
using Application.Contracts.Surveys;

namespace MindWaveAPI.Controllers;

[ApiController]
[Route("api/surveys")]
public sealed class SurveysController : ControllerBase
{
    private readonly ISurveyService _surveyService;
    private readonly ILogger<SurveysController> _logger;

    public SurveysController(ISurveyService surveyService, ILogger<SurveysController> logger)
    {
        _surveyService = surveyService;
        _logger = logger;
    }

    // Step 1: submit first four answers and receive next path and 7 follow-up question IDs
    [HttpPost("{template}/initial")]
    [Authorize]
    [ProducesResponseType(typeof(SubmitInitialAnswersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitInitial(string template, [FromBody] SubmitInitialAnswersRequest request, CancellationToken ct)
    {
        try
        {
            // If template != "daily", handle other templates, or remove this if not needed.
            if (template != "daily")
            {
                return NotFound();
            }

            var response = await _surveyService.SubmitInitialAnswersAsync(request, ct);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ToProblemDetails(ex.Message));
        }
    }

    // Step 2: submit 7 follow-up answers based on selected path
    [HttpPost("daily/followup")]
    [Authorize]
    [ProducesResponseType(typeof(SubmitFollowupAnswersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitFollowup([FromBody] SubmitFollowupAnswersRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _surveyService.SubmitFollowupAnswersAsync(request, ct);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ToProblemDetails(ex.Message));
        }
    }

    private static ProblemDetails ToProblemDetails(string detail)
    {
        return new ProblemDetails
        {
            Title = "Request failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail
        };
    }
}