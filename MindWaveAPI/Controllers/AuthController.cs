using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Auth;
using Application.Contracts.Common;
using Application.Abstractions.Auth;

namespace MindWaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILoginService loginService, ILogger<AuthController> logger)
    {
        _loginService = loginService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _loginService.LoginAsync(request, ct);

        return result switch
        {
            Success<LoginResponse> s => Ok(s.Value),
            Failure f when f.Code == ErrorCodes.Validation => BadRequest(ToProblemDetails(f)),
            Failure f when f.Code == ErrorCodes.Unauthorized => Unauthorized(ToProblemDetails(f)),
            Failure f => StatusCode(StatusCodes.Status500InternalServerError, ToProblemDetails(f))
        };
    }

    private static ProblemDetails ToProblemDetails(Failure failure)
    {
        return new ProblemDetails
        {
            Title = "Request failed",
            Status = failure.Code switch
            {
                ErrorCodes.Validation => StatusCodes.Status400BadRequest,
                ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            },
            Detail = failure.Message
        };
    }
}