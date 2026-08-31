using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Domain.Shared;

namespace Application.Features.Identity.Commands;
public sealed record LogoutRequestCommand(
    string accessToken)
    : ICommand;

internal sealed class LogoutRequestCommandHandler(
    IAuthService authService, IUser user)
    : ICommandHandler<LogoutRequestCommand>
{
    public async Task<Result> Handle(LogoutRequestCommand request, CancellationToken cancellationToken)
    {
        return await authService.Logout(user.Id, request.accessToken);
    }
}
