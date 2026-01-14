using System.Net.Http.Headers;
public class TokenAuthorizationHandler : DelegatingHandler
{
    private readonly JwtAuthenticationStateProvider _authProvider;

    public TokenAuthorizationHandler(JwtAuthenticationStateProvider authProvider)
    {
        _authProvider = authProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authProvider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}