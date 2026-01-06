using ControlPanel.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class TokenStorage : ITokenStorage
{
    private const string TokenKey = "jwt_token";
    private readonly ProtectedLocalStorage _storage;

    public TokenStorage(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public async Task<string?> GetTokenAsync()
    {
        // Solo se llama cuando JS está disponible
        var result = await _storage.GetAsync<string>(TokenKey);
        return result.Success ? result.Value : null;
    }

    public async Task SetTokenAsync(string token)
    {
        await _storage.SetAsync(TokenKey, token);
    }

    public async Task RemoveTokenAsync()
    {
        await _storage.DeleteAsync(TokenKey);
    }
}