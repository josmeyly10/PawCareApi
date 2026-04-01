namespace PawCareApi.models
{
    public class Pet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;   
        public int Age { get; set; }
        
       
        public string? PhotoUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public Guid OwnerId { get; set; }
        public Owner Owner { get; set; } = null!;

      
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
