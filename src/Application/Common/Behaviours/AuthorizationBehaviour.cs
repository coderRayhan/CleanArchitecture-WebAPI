﻿using Application.Common.Abstractions.Identity;
using Application.Common.Exceptions;
using Application.Common.Security;
using Domain.Shared;
using MediatR;
using System.Reflection;

namespace Application.Common.Behaviours;
internal sealed class AuthorizationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(IUser user, IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            if(_user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = 
                authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await _identityService.IsInRoleAsync(_user.Id, role.Trim());

                        if (isInRole.IsSuccess)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

            if (authorizeAttributesWithPolicies.Any())
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _identityService.AuthorizeAsync(_user.Id, policy);

                    if (authorized.IsFailure)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }

        // User is authorized / authorization not required
        return await next().ConfigureAwait(false);
    }
}
