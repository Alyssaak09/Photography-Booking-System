namespace Photography_Booking_System.Models.ViewModels
{
    public class BookingFormViewModel
    {
        // Form Fields
        public int? BookingId { get; set; } 
        public DateTime BookingDate { get; set; }
        public string? Location { get; set; }

        public int ClientId { get; set; }
        public int PhotographerId { get; set; }

        // Multi-select Services
        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        // Dropdown/population sources
        public IEnumerable<Client> Clients { get; set; } = new List<Client>();
        public IEnumerable<Photographer> Photographers { get; set; } = new List<Photographer>();
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
    }
}
