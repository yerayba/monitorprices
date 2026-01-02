using System.Net.Http.Json;
using Control_Panel.Components.Models;

public class AuthService
{
    private readonly IHttpClientFactory _factory;

    public AuthService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PostAsJsonAsync(
            "api/auth/login",
            new { email, password });

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<LoginModel>();
        return result!.Token;
    }
}
