using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MonitorPrices.Domain.Entities;
using MonitorPrices.Repository;

namespace MonitorPrices.Services.Services
{
    
    public class PriceMonitorService : IPriceMonitorService
    {
        private readonly MonitorPricedbContext _db;
        private readonly OutscraperPriceProvider _priceProvider;

        public PriceMonitorService(
            MonitorPricedbContext db,
            OutscraperPriceProvider priceProvider)
        {
            _db = db;
            _priceProvider = priceProvider;
        }

        // Actualiza todos los productos
        public async Task UpdateAllPricesAsync(CancellationToken ct)
        {
            var products = await _db.Products.ToListAsync(ct);
            foreach (var product in products)
            {
                await UpdatePriceAsync(product);
            }
        }

        // Actualiza el precio de un producto específico
        public async Task UpdatePriceAsync(Productos product)
        {
            var newPrice = await _priceProvider.GetPriceAsync(product.Asin);
            if (newPrice.HasValue && newPrice != product.PrecioActual)
            {
                product.PrecioUltimo = product.PrecioActual;
                product.PrecioActual = newPrice;
                product.FechaUltimaComprobacion = DateTime.UtcNow;

                _db.Products.Update(product);
                await _db.SaveChangesAsync();
            }
        }
    }

    // Provider que llama a Outscraper
    public class OutscraperPriceProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public OutscraperPriceProvider(IHttpClientFactory httpClientFactory, string apiKey)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = apiKey;
        }

        public async Task<decimal?> GetPriceAsync(string asin)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var body = new { queries = new[] { asin }, limit = 1 };

            try
            {
                var response = await client.PostAsJsonAsync(
                    "https://api.outscraper.com/amazon-products/v1", body);

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<OutscraperResponse>();
                return data?.Results?.FirstOrDefault()?.Price;
            }
            catch
            {
                return null;
            }
        }

        private class OutscraperResponse
        {
            [JsonPropertyName("results")]
            public List<OutscraperResult>? Results { get; set; }
        }

        private class OutscraperResult
        {
            [JsonPropertyName("price")]
            public decimal? Price { get; set; }
        }
    }
}
