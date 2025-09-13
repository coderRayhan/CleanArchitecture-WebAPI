using API.Extensions;
using API.Infrastructure;
using Application.Features.Identity.Commands;
using Application.Features.Identity.Models;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Identity;

public class Auth : EndpointGroupBase
{
    private static readonly string RefreshTokenKey = "X-Refresh-Token";
    private static readonly string Authorization = nameof(Authorization);
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("Login", Login)
            .WithName("Login")
            .AllowAnonymous()
            .Produces<AuthenticatedResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("RefreshToken", RefreshToken)
            .WithName("RefreshToken")
            .AllowAnonymous()
            .Produces<AuthenticatedResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("Logout", Logout)
            .WithName("Logout")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private async Task<IResult> Login(
        ISender sender,
        IHttpContextAccessor httpContext,
        LoginRequestCommand command)
    {
        Result<AuthenticatedResponse> result = await sender.Send(command);

        if (result.IsSuccess)
        {
            SetRefreshTokenInCookie(httpContext, result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);
        }

        return result.Match(
            onSuccess: () => TypedResults.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> RefreshToken(
        IHttpContextAccessor httpContext,
        ISender sender)
    {
        if(!httpContext.HttpContext.Request.Headers.TryGetValue(Authorization, out var authorizationHeader))
        {
            return TypedResults.BadRequest("Invalid Token");
        }

        if(!httpContext.HttpContext.Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken))
        {
            return TypedResults.BadRequest("Invalid Token");
        }

        var accessToken = authorizationHeader.ToString()
            .Replace("Bearer", "");

        var result = await sender.Send(new RefreshTokenRequestCommand(accessToken, refreshToken));

        if(result.IsFailure) return result.ToProblemDetails();

        SetRefreshTokenInCookie(httpContext, result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);

        return result.Match(
            onSuccess: () => TypedResults.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Logout(IHttpContextAccessor context, ISender sender)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(Authorization, out var authorizationHeader))
        {
            return TypedResults.BadRequest("Invalid Token");
        }

        var accessToken = authorizationHeader.ToString()
            .Replace("Bearer", "");

        var result = await sender.Send(new LogoutRequestCommand(accessToken));

        SetRefreshTokenInCookie(context, string.Empty, DateTime.Now);

        return result.Match(
            onSuccess: () => TypedResults.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private static void SetRefreshTokenInCookie(
        IHttpContextAccessor httpContext,
        string refreshToken,
        DateTime expiresOn)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = expiresOn,
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        httpContext.HttpContext.Response.Cookies.Append(RefreshTokenKey, refreshToken, cookieOptions);
    }
}
