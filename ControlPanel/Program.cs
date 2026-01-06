using ControlPanel.Components;
using ControlPanel.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<ITokenStorage, TokenStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();


// HttpClient
builder.Services.AddHttpClient();

// 🔐 Authentication (OBLIGATORIO para [Authorize])
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
            ValidateLifetime = false, // 👈 IMPORTANTE en Blazor Server
            ValidateIssuerSigningKey = false
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Blazor auth state
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

// Token storage
builder.Services.AddScoped<ITokenStorage, TokenStorage>();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// 🔐 MUY IMPORTANTE: ORDEN CORRECTO
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();