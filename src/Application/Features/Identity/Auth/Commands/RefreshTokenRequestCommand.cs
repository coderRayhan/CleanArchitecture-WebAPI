using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Application.Features.Identity.Models;
using Domain.Shared;

namespace Application.Features.Identity.Commands;
public sealed record RefreshTokenRequestCommand(
    string AccessToken, string RefreshToken)
    : ICommand<AuthenticatedResponse>;

internal sealed class RefreshTokenRequestCommandHandler(
    IAuthService authService)
    : ICommandHandler<RefreshTokenRequestCommand, AuthenticatedResponse>
{
    public async Task<Result<AuthenticatedResponse>> Handle(RefreshTokenRequestCommand request, CancellationToken cancellationToken)
    {
        return await authService.RefreshToken(request.AccessToken, request.RefreshToken, cancellationToken); 
    }
}