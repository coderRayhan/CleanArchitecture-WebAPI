using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Application.Features.Identity.Models;
using Domain.Shared;

namespace Application.Features.Identity.Commands;
public sealed record LoginRequestCommand(
    string UserName,
    string Password,
    bool IsRemember)
    : ICommand<AuthenticatedResponse>;

internal sealed class LoginRequestCommandHandler(
    IAuthService authService)
    : ICommandHandler<LoginRequestCommand, AuthenticatedResponse>
{
    public async Task<Result<AuthenticatedResponse>> Handle(LoginRequestCommand request, CancellationToken cancellationToken)
    {
        return await authService
            .Login(request.UserName, request.Password, cancellationToken);
    }
}
