
namespace PawCareApi.models
{
    
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Date { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public decimal TotalPrice { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public Guid PetId { get; set; }
        public Pet Pet { get; set; } = null!;

        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }

    public class AppointmentService
    {
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }

    public enum AppointmentStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        Transfer,
        Card
    }





}
