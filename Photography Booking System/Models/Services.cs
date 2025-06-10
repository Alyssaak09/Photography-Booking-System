using System.ComponentModel.DataAnnotations;
namespace Photography_Booking_System.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }

        // Many-to-Many: Service can be in many Bookings
        public ICollection<Booking_Service> BookingServices { get; set; } = new List<Booking_Service>();
    }

}
