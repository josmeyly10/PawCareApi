using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.data;
using PawCareApi.DTOs;
using PawCareApi.models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/v1/pets")]
public class PetsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PetsController(AppDbContext db) => _db = db;

   
    [HttpPost]
    public async Task<ActionResult<PetResponse>> Create([FromBody] CreatePetRequest req)
    {
        var ownerExists = await _db.Owners.AnyAsync(o => o.Id == req.OwnerId);
        if (!ownerExists)
            return BadRequest(new { message = "El dueño indicado no existe" });

        var pet = new Pet
        {
            Name = req.Name,
            Breed = req.Breed,
            Age = req.Age,
            OwnerId = req.OwnerId,
            PhotoUrl = req.PhotoUrl
        };

        _db.Pets.Add(pet);
        await _db.SaveChangesAsync();

        var petCount = await _db.Pets.CountAsync(p => p.OwnerId == req.OwnerId);
        if (petCount >= 3)
        {
            var owner = await _db.Owners.FindAsync(req.OwnerId);
            if (owner is not null && !owner.IsVip)
            {
                owner.IsVip = true;
                await _db.SaveChangesAsync();
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, await FetchResponse(pet.Id));
    }

    
    [HttpGet]
    public async Task<ActionResult<List<PetResponse>>> GetAll([FromQuery] string? search)
    {
        var query = _db.Pets.Include(p => p.Owner).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Breed.Contains(search) ||
                p.Owner.FullName.Contains(search));

        var pets = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return Ok(pets.Select(ToResponse).ToList());
    }

    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PetResponse>> GetById(Guid id)
    {
        var pet = await _db.Pets.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null)
            return NotFound(new { message = "Mascota no encontrada" });

        return Ok(ToResponse(pet));
    }

    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PetResponse>> Update(Guid id, [FromBody] UpdatePetRequest req)
    {
        var pet = await _db.Pets.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null)
            return NotFound(new { message = "Mascota no encontrada" });

        if (req.Name is not null) pet.Name = req.Name;
        if (req.Breed is not null) pet.Breed = req.Breed;
        if (req.Age is not null) pet.Age = req.Age.Value;
        if (req.PhotoUrl is not null) pet.PhotoUrl = req.PhotoUrl;

        await _db.SaveChangesAsync();
        return Ok(ToResponse(pet));
    }

    
    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<List<AppointmentResponse>>> GetHistory(Guid id)
    {
        var exists = await _db.Pets.AnyAsync(p => p.Id == id);
        if (!exists) return NotFound(new { message = "Mascota no encontrada" });

        var appointments = await _db.Appointments
            .Include(a => a.Pet).ThenInclude(p => p.Owner)
            .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
            .Where(a => a.PetId == id && a.Status == AppointmentStatus.Completed)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(appointments.Select(AppointmentsController.ToResponse).ToList());
    }

    
    internal static PetResponse ToResponse(Pet p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Breed = p.Breed,
        Age = p.Age,
        PhotoUrl = p.PhotoUrl,
        CreatedAt = p.CreatedAt,
        Owner = new OwnerSummaryResponse
        {
            Id = p.Owner.Id,
            FullName = p.Owner.FullName,
            Phone = p.Owner.Phone,
            IsVip = p.Owner.IsVip
        }
    };

    
    internal static PetSummaryResponse ToSummary(Pet p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Breed = p.Breed,
        Age = p.Age,
        PhotoUrl = p.PhotoUrl
    };

    private async Task<PetResponse> FetchResponse(Guid id)
    {
        var pet = await _db.Pets.Include(p => p.Owner).FirstAsync(p => p.Id == id);
        return ToResponse(pet);
    }
}