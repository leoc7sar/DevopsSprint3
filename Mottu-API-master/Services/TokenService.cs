using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Mottu.Api.Domain;

namespace Mottu.Api.Services;

public class TokenService : ITokenService
{
  private readonly IConfiguration _cfg;
  public TokenService(IConfiguration cfg) { _cfg = cfg; }

  public string Generate(Usuario u)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
      new Claim(ClaimTypes.Name, u.Email),
      new Claim("uid", u.Id.ToString()),
      new Claim("nome", u.Nome)
    };

    var token = new JwtSecurityToken(
      issuer: "mottu",
      audience: "mottu",
      claims: claims,
      expires: DateTime.UtcNow.AddDays(7),
      signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
