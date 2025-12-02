using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Common;
using Application.Contracts.Auth;

namespace Application.Abstractions.Auth;

public interface IRegistrationService
{
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
}