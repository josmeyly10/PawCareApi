namespace PawCareApi.DTOs
{
    

    public class CreateAppointmentRequest
    {
        public Guid PetId { get; set; }
        public List<Guid> ServiceIds { get; set; } = new();
        public string Date { get; set; } = string.Empty;  
        public string TimeSlot { get; set; } = string.Empty;  
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentStatusRequest
    {
        
        public string Status { get; set; } = string.Empty;
       
        public string? PaymentMethod { get; set; }
    }

    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public string Date { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public PetSummaryResponse Pet { get; set; } = null!;
        public List<ServiceResponse> Services { get; set; } = new();
    }

    public class ServiceResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }
}
