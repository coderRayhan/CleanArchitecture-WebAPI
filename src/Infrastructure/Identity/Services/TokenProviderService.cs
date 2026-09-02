
using Infrastructure.Identity.Model;
using Infrastructure.Identity.OptionsSetup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Infrastructure.Identity.Permissions;

namespace Infrastructure.Identity.Services;
internal sealed class TokenProviderService(
    IOptionsSnapshot<JwtOptions> jwtOptions,
    UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : ITokenProviderService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    public async Task<(string AccessToken, int ExpiresInMinutes)> GenerateAccessTokenAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        var roles = await userManager.GetRolesAsync(user!);

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user!.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", user.UserName!),
            new Claim("ip", GetIpAddress())
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var roleClaimTasks = roles.Select(async roleName =>
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null) return Enumerable.Empty<Claim>();

            var roleClaims = await roleManager.GetClaimsAsync(role);
            return roleClaims.Where(c => c.Type == CustomClaimTypes.Permission);
        });
        
        var roleClaimResults = await Task.WhenAll(roleClaimTasks);
        claims.AddRange(roleClaimResults.SelectMany(c => c));

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var accessToken = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            null,
            DateTime.Now.AddMinutes(_jwtOptions.DurationInMinutes),
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(accessToken);

        return (tokenValue, _jwtOptions.DurationInMinutes);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private static string GetIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return string.Empty;
    }
}
