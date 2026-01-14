using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitorPrices.Domain.Entities;
using MonitorPrices.Repository;


namespace MonitorPrices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitorPricesController : ControllerBase
    {
        private readonly MonitorPricedbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public MonitorPricesController(MonitorPricedbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _dbContext.Products.ToListAsync();
            return Ok(products);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            product.FechaUltimaComprobación = DateTime.UtcNow;
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }

        // PUT: api/products/{id}/update-price
        [HttpPut("{id}/update-price")]
        public async Task<IActionResult> UpdatePrice(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var newPrice = await GetAmazonPriceAsync(product.PrecioActual);
            if (newPrice == null)
                return BadRequest("No se pudo obtener el precio");

            product.PrecioUltimo = product.PrecioActual;
            product.PrecioActual = newPrice;
            product.FechaUltimaComprobación = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        private async Task<string?> GetAmazonPriceAsync(string asin)
        {
            // Esto es un ejemplo, reemplaza con tu llamada real a la API de Amazon
            var client = _httpClientFactory.CreateClient();
            // Suponiendo que tu API de Amazon devuelve JSON con { "price": 123.45 }
            var response = await client.GetFromJsonAsync<AmazonPriceResponse>($"https://api.amazon.fake/product/{asin}");
            return response?.Price;
        }

        private class AmazonPriceResponse
        {
            public string Price { get; set; }
        }
    }
}
