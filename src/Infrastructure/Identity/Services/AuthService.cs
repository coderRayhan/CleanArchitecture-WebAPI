using Application.Common.Abstractions.Identity;
using Application.Common.Constants;
using Application.Features.Identity.Models;
using Domain.Shared;
using Infrastructure.Identity.Model;
using Infrastructure.Identity.OptionsSetup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.Services;
internal sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    ITokenProviderService tokenProvider,
    IdentityContext context,
    IOptionsSnapshot<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    public async Task<Result<AuthenticatedResponse>> Login(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByNameAsync(username)
            ?? await userManager.FindByEmailAsync(username);

        if (user is null)
            return Result.Failure<AuthenticatedResponse>(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        var result = await userManager.CheckPasswordAsync(user, password);

        if(!result)
            return Result.Failure<AuthenticatedResponse>(Error.NotFound(nameof(user), ErrorMessages.WRONG_USERNAME_PASSWORD));

        var (accessToken, expiresInMinute) = await tokenProvider.GenerateAccessTokenAsync(user.Id);

        var authResponse = new AuthenticatedResponse
        {
            AccessToken = accessToken,
            ExpiresInMinutes = expiresInMinute
        };

        var lastRefreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id, cancellationToken);

        if(lastRefreshToken is { IsActive: true })
        {
            authResponse.RefreshToken = lastRefreshToken.Token;
            authResponse.RefreshTokenExpiresOn = lastRefreshToken.Expires;

            return Result.Success(authResponse);
        }

        var refreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            Expires = DateTime.Now.AddDays(_jwtOptions.RefreshTokenExpires),
            ApplicationUserId = user.Id
        };

        authResponse.RefreshToken = refreshToken.Token;
        authResponse.RefreshTokenExpiresOn = refreshToken.Expires;

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        return !string.IsNullOrEmpty(accessToken)
            ? Result.Success(authResponse)
            : Result.Failure<AuthenticatedResponse>(Error.NotFound(nameof(user), ErrorMessages.WRONG_USERNAME_PASSWORD));
    }
    public Task<(Result Result, string UserId)> ForgotPassword(string email)
    {
        throw new NotImplementedException();
    }


    public async Task<Result<AuthenticatedResponse>> RefreshToken(
        string accessToken, 
        string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        var existedRefreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        //existedRefreshToken is null means the invalid token
        if (existedRefreshToken is null)
            return Result.Failure<AuthenticatedResponse>(Error.Validation("Token", ErrorMessages.TOKEN_DID_NOT_MATCH));

        //Checking status of the token
        if (!existedRefreshToken.IsActive)
            return Result.Failure<AuthenticatedResponse>(Error.Failure("Token", ErrorMessages.TOKEN_NOT_ACTIVE));

        //Revoke current refresh token
        existedRefreshToken.Revoked = DateTime.Now;

        //get claims principle from access token
        var claimPrinciple = GetClaimsPrincipleFromJwtToken(accessToken);

        if (claimPrinciple.IsFailure)
            return Result.Failure<AuthenticatedResponse>(claimPrinciple.Error);

        //get userId from principle
        var userId = claimPrinciple.Value.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new SecurityTokenException(ErrorMessages.INVALID_TOKEN);

        //generate new access token
        var (token, expiresInMinutes) = await tokenProvider.GenerateAccessTokenAsync(userId);

        //generate new refreshToken
        var newRefreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpires),
            Created = DateTime.UtcNow,
            ApplicationUserId = userId
        };

        var tokenResponse = new AuthenticatedResponse
        {
            AccessToken = token,
            ExpiresInMinutes = expiresInMinutes,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresOn = newRefreshToken.Expires
        };

        //Save new refreshToken into db
        context.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync();

        return !string.IsNullOrEmpty(token)
            ? Result.Success(tokenResponse)
            : Result.Failure<AuthenticatedResponse>(Error.NotFound("Token", ErrorMessages.INVALID_TOKEN));
    }

    public Task<(Result Result, string UserId)> ResetPassword(string email)
    {
        throw new NotImplementedException();
    }

    private Result<ClaimsPrincipal> GetClaimsPrincipleFromJwtToken(string jwtToken)
    {
        try
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principle = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out SecurityToken securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Result.Failure<ClaimsPrincipal>(Error.Validation("Token", ErrorMessages.INVALID_TOKEN));
            }

            return principle;
        }
        catch(Exception e)
        {
            return Result.Failure<ClaimsPrincipal>(Error.Validation("Token", ErrorMessages.INVALID_TOKEN));
        }
    }

    public async Task<Result> Logout(string userId, string accessToken, CancellationToken cancellationToken = default)
    {
        // Get ClaimsPrinciple from accessToken
        var claimsPrincipleResult =
            GetClaimsPrincipleFromJwtToken(accessToken);

        // Get Identity UserId from ClaimPrinciple
        var userIdFromPrinciple = claimsPrincipleResult.Value?.FindFirstValue(ClaimTypes.NameIdentifier);

        context.RefreshTokens
            .Where(x => x.ApplicationUserId == (userId ?? userIdFromPrinciple) && x.Revoked == null)
            .ExecuteUpdate(e => e.SetProperty(e => e.Revoked, DateTime.Now));

        var affectedRow = await context.SaveChangesAsync(cancellationToken);

        return affectedRow > 0
            ? Result.Success()
            : Result.Failure(Error.NotFound("Token", ErrorMessages.INVALID_TOKEN));
    }
}
