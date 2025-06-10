
namespace Photography_Booking_System.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Location { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; } = null!;

        public int PhotographerId { get; set; }
        public string PhotographerName { get; set; } = null!;

        public List<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
    }
}
