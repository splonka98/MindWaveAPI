namespace Application.Contracts.Auth;

public sealed class RegisterRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = "Patient"; // "Patient" or "Doctor"
}