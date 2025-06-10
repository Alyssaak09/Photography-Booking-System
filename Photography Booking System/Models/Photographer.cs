using System.ComponentModel.DataAnnotations;
namespace Photography_Booking_System.Models
{
    public class Photographer
    {
        [Key]
        public int PhotographerId { get; set; }
        public required string Name { get; set; }
        public required string Specialty { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
