using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.data;
using PawCareApi.DTOs;
using PawCareApi.models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/v1/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AppointmentsController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Create([FromBody] CreateAppointmentRequest req)
    {
        var petExists = await _db.Pets.AnyAsync(p => p.Id == req.PetId);
        if (!petExists)
            return BadRequest(new { message = "La mascota indicada no existe" });

        var conflict = await _db.Appointments.AnyAsync(a =>
            a.Date == req.Date &&
            a.TimeSlot == req.TimeSlot &&
            a.Status != AppointmentStatus.Cancelled);

        if (conflict)
            return Conflict(new { message = "Ya existe una cita en ese horario" });

        var services = await _db.Services
            .Where(s => req.ServiceIds.Contains(s.Id))
            .ToListAsync();

        if (services.Count != req.ServiceIds.Count)
            return BadRequest(new { message = "Uno o más servicios no existen" });

        var appointment = new Appointment
        {
            PetId = req.PetId,
            Date = req.Date,
            TimeSlot = req.TimeSlot,
            TotalPrice = services.Sum(s => s.Price),
            Notes = req.Notes,
            AppointmentServices = services.Select(s => new AppointmentService
            {
                ServiceId = s.Id
            }).ToList()
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id },
            await FetchResponse(appointment.Id));
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentResponse>>> GetAll(
        [FromQuery] string? date,
        [FromQuery] string? status)
    {
        var query = _db.Appointments
            .Include(a => a.Pet).ThenInclude(p => p.Owner)
            .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(date))
            query = query.Where(a => a.Date == date);

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<AppointmentStatus>(status, true, out var parsed))
            query = query.Where(a => a.Status == parsed);

        var list = await query.OrderBy(a => a.Date).ThenBy(a => a.TimeSlot).ToListAsync();
        return Ok(list.Select(ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(Guid id)
    {
        var appt = await _db.Appointments
            .Include(a => a.Pet).ThenInclude(p => p.Owner)
            .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appt is null)
            return NotFound(new { message = "Cita no encontrada" });

        return Ok(ToResponse(appt));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<AppointmentResponse>> UpdateStatus(
        Guid id, [FromBody] UpdateAppointmentStatusRequest req)
    {
        var appt = await _db.Appointments
            .Include(a => a.Pet).ThenInclude(p => p.Owner)
            .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appt is null)
            return NotFound(new { message = "Cita no encontrada" });

        if (!Enum.TryParse<AppointmentStatus>(req.Status, true, out var newStatus))
            return BadRequest(new { message = "Estado inválido. Usa: Pending, InProgress, Completed, Cancelled" });

        appt.Status = newStatus;

        if (newStatus == AppointmentStatus.Completed && req.PaymentMethod is not null)
        {
            if (Enum.TryParse<PaymentMethod>(req.PaymentMethod, true, out var pm))
                appt.PaymentMethod = pm;
        }

        await _db.SaveChangesAsync();
        return Ok(ToResponse(appt));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var appt = await _db.Appointments
            .Include(a => a.AppointmentServices)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appt is null)
            return NotFound(new { message = "Cita no encontrada" });

        if (appt.Status == AppointmentStatus.InProgress)
            return Conflict(new { message = "No se puede eliminar una cita en progreso" });

        _db.Appointments.Remove(appt);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    internal static AppointmentResponse ToResponse(Appointment a) => new()
    {
        Id = a.Id,
        Date = a.Date,
        TimeSlot = a.TimeSlot,
        Status = a.Status.ToString(),
        TotalPrice = a.TotalPrice,
        PaymentMethod = a.PaymentMethod?.ToString(),
        Notes = a.Notes,
        CreatedAt = a.CreatedAt,
        Pet = new PetSummaryResponse
        {
            Id = a.Pet.Id,
            Name = a.Pet.Name,
            Breed = a.Pet.Breed,
            Age = a.Pet.Age,
            PhotoUrl = a.Pet.PhotoUrl
        },
        Services = a.AppointmentServices.Select(s => new ServiceResponse
        {
            Id = s.Service.Id,
            Name = s.Service.Name,
            Description = s.Service.Description,
            Price = s.Service.Price,
            DurationMinutes = s.Service.DurationMinutes
        }).ToList()
    };

    private async Task<AppointmentResponse> FetchResponse(Guid id)
    {
        var a = await _db.Appointments
            .Include(a => a.Pet).ThenInclude(p => p.Owner)
            .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
            .FirstAsync(a => a.Id == id);
        return ToResponse(a);
    }
}