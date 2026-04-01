namespace PawCareApi.models
{
    public class Owner
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsVip { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
