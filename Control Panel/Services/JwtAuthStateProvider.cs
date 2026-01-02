using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private ClaimsPrincipal? _user; // Estado actual en memoria
    private string? _token;         // Token JWT en memoria

    // Devuelve el estado actual de autenticación
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_user ?? _anonymous));
    }

    // Setea el token tras login exitoso
    public void SetToken(string token)
    {
        _token = token;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Opcional: validar expiración
        if (jwt.ValidTo < DateTime.UtcNow)
        {
            Logout();
            return;
        }

        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        _user = new ClaimsPrincipal(identity);

        // Notificar a Blazor que el estado cambió
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    // Logout
    public void Logout()
    {
        _user = null;
        _token = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    // Devuelve token si lo necesitas para llamadas HTTP
    public string? GetToken() => _token;
}