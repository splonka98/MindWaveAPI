using Application.Contracts.Auth;
using Application.Contracts.Common;

namespace Application.Abstractions.Auth;

public interface ILoginService
{
    Task<Result> LoginAsync(LoginRequest request, CancellationToken ct);
}