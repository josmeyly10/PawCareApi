using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.data;
using PawCareApi.DTOs;
using PawCareApi.models;

namespace PawCareApi.Controllers;


[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll([FromQuery] string? category)
    {
        var query = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.ToLower() == category.ToLower());

        var products = await query.OrderBy(p => p.Name).ToListAsync();
        return Ok(products.Select(ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product is null)
            return NotFound(new { message = "Producto no encontrado" });

        return Ok(ToResponse(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest req)
    {
        var product = new Product
        {
            Name = req.Name,
            Category = req.Category,
            Price = req.Price,
            ImageUrl = req.ImageUrl,
            Stock = req.Stock
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToResponse(product));
    }

    [HttpPatch("{id:guid}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] int newStock)
    {
        var product = await _db.Products.FindAsync(id);

        if (product is null)
            return NotFound(new { message = "Producto no encontrado" });

        product.Stock = newStock;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Stock actualizado", stock = newStock });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product is null)
            return NotFound(new { message = "Producto no encontrado" });

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static ProductResponse ToResponse(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Category = p.Category,
        Price = p.Price,
        ImageUrl = p.ImageUrl,
        Stock = p.Stock
    };
}