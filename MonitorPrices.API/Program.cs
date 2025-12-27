using Microsoft.EntityFrameworkCore;
using MonitorPrices.Repository;

var builder = WebApplication.CreateBuilder(args);

// DEBUG: Verificar qué configuración está cargada
Console.WriteLine("=== DEBUG CONFIGURACIÓN ===");
Console.WriteLine($"ConnectionStrings:DefaultConnection = '{builder.Configuration.GetConnectionString("DefaultConnection")}'");
Console.WriteLine($"ConnectionStrings__DefaultConnection = '{Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")}'");

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OBTENER CONEXIÓN CON FALLBACK
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? throw new InvalidOperationException("No se encontró ConnectionStrings:DefaultConnection ni variable de entorno");

Console.WriteLine($"Usando connection string: {connectionString}");

builder.Services.AddDbContext<MonitorPricedbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// MIGRACIONES CON TRY-CATCH Y LOG
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<MonitorPricedbContext>();
try 
{
    Console.WriteLine("Aplicando migraciones...");
    context.Database.Migrate();
    Console.WriteLine("✅ Migraciones aplicadas correctamente");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error en migraciones: {ex.Message}");
    throw;
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "My API Documentation";
        c.DefaultModelsExpandDepth(1);
    });
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();