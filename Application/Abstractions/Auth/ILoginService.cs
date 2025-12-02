using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Common;
using Application.Contracts.Auth;

namespace Application.Abstractions.Auth;

public interface ILoginService
{
    Task<Result> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}