namespace Photography_Booking_System.DTOs
{
    public class BookingSummaryDTO
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string PhotographerName { get; set; } = string.Empty;
        public int ServiceCount { get; set; }
    }
}
