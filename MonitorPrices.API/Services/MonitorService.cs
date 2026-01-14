using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorPrices.Repository;

namespace MonitorPrices.Services
{
    public class PriceMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30); // cada 30 minutos

        public PriceMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateAllPricesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar precios: {ex.Message}");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task UpdateAllPricesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MonitorPricedbContext>();
            var httpFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            var products = await dbContext.Products.ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                string? newPrice = await GetAmazonPriceAsync(httpFactory, product.PrecioActual);
                if (string.IsNullOrEmpty(newPrice) && newPrice != product.PrecioActual)
                {
                    product.PrecioUltimo = product.PrecioActual;
                    product.PrecioActual = newPrice;
                    product.FechaUltimaComprobación = DateTime.UtcNow;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<string?> GetAmazonPriceAsync(IHttpClientFactory httpFactory, string asin)
        {
            var client = httpFactory.CreateClient();
            try
            {
                // Simulación de llamada a la API de Amazon
                // Reemplaza con tu endpoint real
                var response = await client.GetFromJsonAsync<AmazonPriceResponse>(
                    $"https://api.amazon.fake/product/{asin}");
                return response.Price.ToString();
            }
            catch
            {
                return null;
            }
        }

        private class AmazonPriceResponse
        {
            public decimal Price { get; set; }
        }
    }
}
