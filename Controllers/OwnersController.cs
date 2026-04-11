using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.data;
using PawCareApi.DTOs;
using PawCareApi.models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/v1/owners")]
public class OwnersController : ControllerBase
{
    private readonly AppDbContext _db;

    public OwnersController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<OwnerResponse>> Create([FromBody] CreateOwnerRequest req)
    {
        var owner = new Owner
        {
            FullName = req.FullName,
            Phone = req.Phone,
            Email = req.Email,
            Address = req.Address
        };

        _db.Owners.Add(owner);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = owner.Id }, ToResponse(owner));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OwnerResponse>> GetById(Guid id)
    {
        var owner = await _db.Owners
            .Include(o => o.Pets)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (owner is null)
            return NotFound(new { message = "Dueño no encontrado" });

        return Ok(ToResponse(owner));
    }

    [HttpGet]
    public async Task<ActionResult<List<OwnerResponse>>> GetAll()
    {
        var owners = await _db.Owners
            .Include(o => o.Pets)
            .OrderBy(o => o.FullName)
            .ToListAsync();

        return Ok(owners.Select(ToResponse).ToList());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OwnerResponse>> Update(Guid id, [FromBody] UpdateOwnerRequest req)
    {
        var owner = await _db.Owners
            .Include(o => o.Pets)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (owner is null)
            return NotFound(new { message = "Dueño no encontrado" });

        if (req.FullName is not null) owner.FullName = req.FullName;
        if (req.Phone is not null) owner.Phone = req.Phone;
        if (req.Email is not null) owner.Email = req.Email;
        if (req.Address is not null) owner.Address = req.Address;

        await _db.SaveChangesAsync();
        return Ok(ToResponse(owner));
    }

    [HttpGet("{id:guid}/pets")]
    public async Task<ActionResult<List<PetSummaryResponse>>> GetPets(Guid id)
    {
        var exists = await _db.Owners.AnyAsync(o => o.Id == id);
        if (!exists)
            return NotFound(new { message = "Dueño no encontrado" });

        var pets = await _db.Pets
            .Where(p => p.OwnerId == id)
            .Select(p => new PetSummaryResponse
            {
                Id = p.Id,
                Name = p.Name,
                Breed = p.Breed,
                Age = p.Age,
                PhotoUrl = p.PhotoUrl
            })
            .ToListAsync();

        return Ok(pets);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var owner = await _db.Owners
            .Include(o => o.Pets)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (owner is null)
            return NotFound(new { message = "Dueño no encontrado" });

        if (owner.Pets.Any())
            return Conflict(new { message = "No se puede eliminar un dueño con mascotas registradas" });

        _db.Owners.Remove(owner);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static OwnerResponse ToResponse(Owner o) => new()
    {
        Id = o.Id,
        FullName = o.FullName,
        Phone = o.Phone,
        Email = o.Email,
        Address = o.Address,
        IsVip = o.IsVip,
        CreatedAt = o.CreatedAt,
        Pets = o.Pets.Select(p => new PetSummaryResponse
        {
            Id = p.Id,
            Name = p.Name,
            Breed = p.Breed,
            Age = p.Age,
            PhotoUrl = p.PhotoUrl
        }).ToList()
    };
}