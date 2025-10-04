using Microsoft.EntityFrameworkCore;
using MunicipalServices.Models;

namespace MunicipalServices.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LocalEvent> LocalEvents { get; set; }
        public DbSet<UserSearchHistory> UserSearchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            });

            // Configure LocalEvent entity
            modelBuilder.Entity<LocalEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).HasMaxLength(200);
            });

            // Configure UserSearchHistory entity
            modelBuilder.Entity<UserSearchHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SearchTerm).HasMaxLength(200);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.SessionId).HasMaxLength(100);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Password = "AdminPass123!",
                    IsAdmin = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed sample events
            modelBuilder.Entity<LocalEvent>().HasData(
                new LocalEvent
                {
                    Id = 1,
                    Title = "Community Clean-Up Day",
                    Description = "Join us for a community-wide clean-up initiative to beautify our neighborhoods.",
                    Category = "Community",
                    EventDate = DateTime.Now.AddDays(7),
                    Location = "City Park, Johannesburg",
                    Priority = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new LocalEvent
                {
                    Id = 2,
                    Title = "Water Safety Workshop",
                    Description = "Learn about water conservation and safety measures for your home.",
                    Category = "Education",
                    EventDate = DateTime.Now.AddDays(14),
                    Location = "Municipal Hall, Cape Town",
                    Priority = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new LocalEvent
                {
                    Id = 3,
                    Title = "Road Maintenance Notice",
                    Description = "Scheduled maintenance on Main Street will affect traffic flow.",
                    Category = "Infrastructure",
                    EventDate = DateTime.Now.AddDays(3),
                    Location = "Main Street, Durban",
                    Priority = 4,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}