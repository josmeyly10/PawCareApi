using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.data;
using PawCareApi.DTOs;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/v1/services")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ServicesController(AppDbContext db) => _db = db;

    
    [HttpGet]
    public async Task<ActionResult<List<ServiceResponse>>> GetAll()
    {
        var services = await _db.Services.ToListAsync();

        return Ok(services.Select(s => new ServiceResponse
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Price = s.Price,
            DurationMinutes = s.DurationMinutes
        }).ToList());
    }

    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceResponse>> GetById(Guid id)
    {
        var s = await _db.Services.FindAsync(id);

        if (s is null)
            return NotFound(new { message = "Servicio no encontrado" });

        return Ok(new ServiceResponse
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Price = s.Price,
            DurationMinutes = s.DurationMinutes
        });
    }
}