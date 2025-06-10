using System.ComponentModel.DataAnnotations;
namespace Photography_Booking_System.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int PhotographerId { get; set; }
        public Photographer Photographer { get; set; } = null!;

        public DateTime BookingDate { get; set; }
        public string? Location { get; set; }

        // Many-to-Many: Booking can include many Services
        public ICollection<Booking_Service> BookingServices { get; set; } = new List<Booking_Service>();
    }

}
