using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using MonitorPrices.Domain.Entities;
using MonitorPrices.Domain.Interfaces;
using MonitorPrices.Services.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public string Login(string email, string password)
    {
        var user = _userRepository.GetByEmail(email);
        if (user == null)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        // Aquí deberías validar hash
        if (user.PasswordHas != password)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        var jwt = _config.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}