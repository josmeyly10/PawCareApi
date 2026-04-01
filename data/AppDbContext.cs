namespace PawCareApi.data
{
   
    using Microsoft.EntityFrameworkCore;
    using PawCareApi.models;

   

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

       
        public DbSet<Owner> Owners => Set<Owner>();
        public DbSet<Pet> Pets => Set<Pet>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<AppointmentService>()
                .HasKey(a => new { a.AppointmentId, a.ServiceId });

            modelBuilder.Entity<AppointmentService>()
                .HasOne(a => a.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(a => a.AppointmentId);

            modelBuilder.Entity<AppointmentService>()
                .HasOne(a => a.Service)
                .WithMany(s => s.AppointmentServices)
                .HasForeignKey(a => a.ServiceId);

            
            modelBuilder.Entity<Appointment>().Property(a => a.TotalPrice).HasPrecision(10, 2);
            modelBuilder.Entity<Service>().Property(s => s.Price).HasPrecision(10, 2);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(10, 2);

           
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                    Name = "Baño Completo",
                    Description = "Shampoo, secado y peinado",
                    Price = 350,
                    DurationMinutes = 60
                },
                new Service
                {
                    Id = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                    Name = "Peluquería",
                    Description = "Corte personalizado de pelo",
                    Price = 480,
                    DurationMinutes = 90
                },
                new Service
                {
                    Id = Guid.Parse("11111111-0000-0000-0000-000000000003"),
                    Name = "Corte de Uñas",
                    Description = "Lima y corte seguro",
                    Price = 150,
                    DurationMinutes = 20
                },
                new Service
                {
                    Id = Guid.Parse("11111111-0000-0000-0000-000000000004"),
                    Name = "Limpieza Dental",
                    Description = "Cepillado y sanitización",
                    Price = 220,
                    DurationMinutes = 30
                }
            );
        }
    }
}
