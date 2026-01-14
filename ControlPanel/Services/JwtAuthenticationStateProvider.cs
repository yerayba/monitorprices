using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ControlPanel.Interfaces;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStorage _tokenStorage;
    private ClaimsPrincipal _user = new ClaimsPrincipal(new ClaimsIdentity());

    public JwtAuthenticationStateProvider(ITokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    // Devuelve el estado actual del usuario
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_user));
    }

    // Llamar desde OnAfterRenderAsync para cargar el token
    public async Task LoadTokenAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            var claims = ParseClaimsFromJwt(token);
            _user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        }
        else
        {
            _user = new ClaimsPrincipal(new ClaimsIdentity());
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
    }

    public async Task LoginAsync(string token)
    {
        await _tokenStorage.SetTokenAsync(token);
        await LoadTokenAsync();
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.RemoveTokenAsync();
        _user = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }

    // Método para obtener token en HttpClient
    public async Task<string> GetTokenAsync() => await _tokenStorage.GetTokenAsync() ?? "";
}