namespace PawCareApi.DTOs
{


    public class CreateOwnerRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

   
    public class UpdateOwnerRequest
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

   
    public class OwnerResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsVip { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PetSummaryResponse> Pets { get; set; } = new();
    }

    
    public class OwnerSummaryResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsVip { get; set; }
    }

    
}
