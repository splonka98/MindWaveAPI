namespace Application.Contracts.Auth;

public sealed class LoginResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}