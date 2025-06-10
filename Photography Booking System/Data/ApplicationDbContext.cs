using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Models;

namespace Photography_Booking_System.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets (Entity tables)
    public DbSet<Client> Clients { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Photographer> Photographers { get; set; }
    public DbSet<Booking_Service> Booking_Services { get; set; }

    // Configures relationships and keys using the EF Core Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Decimal precision for Price in Service
        modelBuilder.Entity<Service>()
            .Property(s => s.Price)
            .HasPrecision(10, 2);

        // Many-to-Many: Booking_Service
        modelBuilder.Entity<Booking_Service>()
            .HasKey(bs => new { bs.BookingId, bs.ServiceId });

        modelBuilder.Entity<Booking_Service>()
            .HasOne(bs => bs.Booking)
            .WithMany(b => b.BookingServices)
            .HasForeignKey(bs => bs.BookingId);

        modelBuilder.Entity<Booking_Service>()
            .HasOne(bs => bs.Service)
            .WithMany(s => s.BookingServices)
            .HasForeignKey(bs => bs.ServiceId);
    }
}
