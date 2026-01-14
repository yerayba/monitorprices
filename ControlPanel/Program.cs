using ControlPanel.Components;
using ControlPanel.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();

// Protected storage
builder.Services.AddScoped<ProtectedLocalStorage>();

// Auth services
builder.Services.AddScoped<ITokenStorage, TokenStorage>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

// Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// HttpClient con DelegatingHandler para enviar token automáticamente
builder.Services.AddTransient<TokenAuthorizationHandler>();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://192.168.0.24:8080"); // Cambia a tu API
})
.AddHttpMessageHandler<TokenAuthorizationHandler>();

// JWT Authentication (necesario para [Authorize] aunque sea Server)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false, // ⚠ Blazor Server normalmente no valida exp
        ValidateIssuerSigningKey = false
    };
});

// Middleware pipeline
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// 🔐 ORDEN IMPORTANTE
app.UseAuthentication();
app.UseAuthorization();

// Mapear App.razor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
