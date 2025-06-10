namespace Photography_Booking_System.Models
{
    public class Booking_Service
    {
        public int BookingId { get; set; }
        
        // Navigation property
        public Booking Booking { get; set; } = null!;  

        public int ServiceId { get; set; }

        // Navigation property
        public Service Service { get; set; } = null!; 
    }
}
