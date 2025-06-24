using Microsoft.AspNetCore.Mvc.Rendering;

namespace Photography_Booking_System.Models.ViewModels
{
    public class BookingServiceViewModel
    {
        public int BookingId { get; set; }
        public int ServiceId { get; set; }

        public IEnumerable<SelectListItem>? Bookings { get; set; }
        public IEnumerable<SelectListItem>? Services { get; set; }
    }
}
