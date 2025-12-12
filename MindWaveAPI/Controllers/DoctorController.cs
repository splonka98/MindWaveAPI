using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Doctors;
using Application.Contracts.Doctors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MindWaveAPI.Controllers;

[ApiController]
[Route("api/doctor")]
[Authorize(Roles = "Doctor")]
public sealed class DoctorController : ControllerBase
{
    private readonly IDoctorPatientService _service;

    public DoctorController(IDoctorPatientService service) => _service = service;

    [HttpPost("pair")]
    public async Task<IActionResult> PairPatient([FromBody] PairPatientRequest request, CancellationToken ct)
    {
        var doctorIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(doctorIdStr, out var doctorId))
        {
            return Unauthorized("Missing or invalid 'sub' claim.");
        }

        await _service.PairAsync(doctorId, request.PatientUserId, ct);
        return NoContent();
    }

    [HttpGet("patient/{patientUserId:guid}/surveys")]
    public async Task<IActionResult> GetPatientSurveys([FromRoute] Guid patientUserId, [FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken ct)
    {
        var doctorIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(doctorIdStr, out var doctorId))
        {
            return Unauthorized("Missing or invalid 'sub' claim.");
        }

        var list = await _service.GetPatientSurveysAsync(doctorId, patientUserId, from, to, ct);
        return Ok(list);
    }

    [HttpGet("patient/{patientUserId:guid}/surveys/answers")]
    public async Task<IActionResult> GetPatientSurveysWithAnswers([FromRoute] Guid patientUserId, [FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken ct)
    {
        var doctorIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(doctorIdStr, out var doctorId))
        {
            return Unauthorized("Missing or invalid 'sub' claim.");
        }

        var list = await _service.GetPatientSurveysWithAnswersAsync(doctorId, patientUserId, from, to, ct);
        return Ok(list);
    }
}