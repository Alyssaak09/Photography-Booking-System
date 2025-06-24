using Photography_Booking_System.DTOs;

namespace Photography_Booking_System.Models.ViewModels
{
    public class BookingDetailsViewModel
    {
        public required BookingDTO Booking { get; set; }
        public IEnumerable<ServiceDTO>? Services { get; set; }
    }
}
