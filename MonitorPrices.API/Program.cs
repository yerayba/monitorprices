using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonitorPrices.Domain.Interfaces;
using MonitorPrices.Repository;
using MonitorPrices.Repository.Repositories;
using MonitorPrices.Services.Services;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("=== DEBUG CONFIGURACIÓN ===");
Console.WriteLine($"ConnectionStrings:DefaultConnection = '{builder.Configuration.GetConnectionString("DefaultConnection")}'");
Console.WriteLine($"ConnectionStrings__DefaultConnection = '{Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")}'");

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? throw new InvalidOperationException("No se encontró ConnectionStrings:DefaultConnection ni variable de entorno");

Console.WriteLine($"Usando connection string: {connectionString}");

builder.Services.AddDbContext<MonitorPricedbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<MonitorPricedbContext>();
try 
{
    Console.WriteLine("Aplicando migraciones...");
    context.Database.Migrate();
    Console.WriteLine("Migraciones aplicadas correctamente");
}
catch (Exception ex)
{
    Console.WriteLine($"Error en migraciones: {ex.Message}");
    throw;
}


app.UseSwagger();
app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "My API Documentation";
        c.DefaultModelsExpandDepth(1);
    });


//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();