using System.ComponentModel.DataAnnotations;
namespace Photography_Booking_System.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }

        //One-to-Many: Client has many Bookings
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
