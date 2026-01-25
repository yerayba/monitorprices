using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitorPrices.Domain.Entities;
using MonitorPrices.Repository;

namespace MonitorPrices.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductosController : ControllerBase
{
    private readonly MonitorPricedbContext _db;
    private readonly IPriceMonitorService _priceMonitorService;

    public ProductosController(
        MonitorPricedbContext db,
        IPriceMonitorService priceMonitorService)
    {
        _db = db;
        _priceMonitorService = priceMonitorService;
    }

    // GET /api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _db.Products.ToListAsync();
        return Ok(products);
    }

    // GET /api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
    public async Task<IActionResult> AddProduct(Productos request)
    {
        var product = new Productos
        {
            Asin = request.Asin,
            Nombre = request.Nombre
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // Opcional: actualizar precio inmediatamente
        await _priceMonitorService.UpdatePriceAsync(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}

    