
using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Domain.Shared;

namespace Application.Features.Identity.Commands;
public sealed record ChangePasswordCommand(
    string oldPassword,
    string newPassword)
    : ICommand;

internal sealed class ChangePasswordCommandHandler(
    IIdentityService identityService,
    IUser user)
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await identityService.ChangePasswordAsync(
            userId: user.Id, 
            oldPassword: request.oldPassword, newPassword: request.newPassword,
            cancellationToken);
    }
}
