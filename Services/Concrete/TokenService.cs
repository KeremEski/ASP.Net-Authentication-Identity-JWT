using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Models;
using Authentication.Services.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services.Concrete;

// Important: Must implement this service. But you can change it or add new mehtods if you want.
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigninKey"]!));
    }

    public string CreateToken(User user)
    {
        // You can edit your own claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Role, "User") 
        };

        // Token creator and signin type.
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(1), // Token validate time.
            SigningCredentials = creds,
            Audience = _config["JWT:Audience"], // Validation for Audience.
            Issuer = _config["JWT:Issuer"] // Validation for Issuer.
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token); // return String Token.
    }

}
