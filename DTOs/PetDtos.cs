namespace PawCareApi.DTOs
{
    
    public class CreatePetRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public Guid OwnerId { get; set; }

       
        public string? PhotoUrl { get; set; }
    }

   
    public class UpdatePetRequest
    {
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public int? Age { get; set; }

        
        public string? PhotoUrl { get; set; }
    }

    
    public class PetResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }

       
        public string? PhotoUrl { get; set; }

        public DateTime CreatedAt { get; set; }

       
        public OwnerSummaryResponse Owner { get; set; } = null!;
    }

  
    public class PetSummaryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }

        
        public string? PhotoUrl { get; set; }
    }
}
