using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.DTOs;
using Photography_Booking_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography_Booking_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Booking_ServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Booking_ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of all booking services.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Booking_Service
        /// -> [{"BookingId":1, "ServiceName":"Wedding Shoot", ...}, {...}]
        /// </example>
        /// <returns>A list of Booking_Service objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking_Service>>> GetBooking_Services()
        {
            return await _context.Booking_Services.ToListAsync();
        }

        /// <summary>
        /// Returns a specific booking service by composite key (BookingId, ServiceId).
        /// </summary>
        /// <param name="bookingId">The Booking ID of the booking service to retrieve.</param>
        /// <param name="serviceId">The Service ID of the booking service to retrieve.</param>
        /// <example>
        /// GET http://localhost:7198/api/Booking_Service/5/10
        /// -> {"BookingId":5, "ServiceId":10, "ServiceName":"Birthday Shoot", ...}
        /// </example>
        /// <returns>A Booking_Service object if found; otherwise, 404 Not Found.</returns>
        [HttpGet("{bookingId}/{serviceId}")]
        public async Task<ActionResult<Booking_Service>> GetBooking_Service(int bookingId, int serviceId)
        {
            var booking_Service = await _context.Booking_Services.FindAsync(bookingId, serviceId);

            if (booking_Service == null)
            {
                return NotFound();
            }

            return booking_Service;
        }

        /// <summary>
        /// Updates an existing booking service identified by composite key (BookingId, ServiceId).
        /// </summary>
        /// <param name="bookingId">The Booking ID of the booking service to update.</param>
        /// <param name="serviceId">The Service ID of the booking service to update.</param>
        /// <param name="booking_Service">The updated booking service object.</param>
        /// <example>
        /// PUT http://localhost:7198/api/Booking_Service/5/10
        /// Body: {"BookingId":5, "ServiceId":10, "ServiceName":"Updated Service", ...}
        /// </example>
        /// <returns>
        /// 204 No Content on successful update,
        /// 400 Bad Request if IDs in URL and body do not match,
        /// 404 Not Found if the booking service does not exist.
        /// </returns>
        [HttpPut("{bookingId}/{serviceId}")]
        public async Task<IActionResult> PutBooking_Service(int bookingId, int serviceId, Booking_Service booking_Service)
        {
            if (bookingId != booking_Service.BookingId || serviceId != booking_Service.ServiceId)
            {
                return BadRequest();
            }

            _context.Entry(booking_Service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Booking_ServiceExists(bookingId, serviceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new booking service.
        /// </summary>
        /// <param name="booking_Service">The booking service object to create.</param>
        /// <example>
        /// POST http://localhost:7198/api/Booking_Service
        /// Body: {"BookingId":10, "ServiceId":3, "ServiceName":"New Service", ...}
        /// </example>
        /// <returns>
        /// 201 Created with the created booking service object,
        /// 409 Conflict if a booking service with the same composite key already exists.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<Booking_Service>> PostBooking_Service(Booking_Service booking_Service)
        {
            _context.Booking_Services.Add(booking_Service);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Booking_ServiceExists(booking_Service.BookingId, booking_Service.ServiceId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBooking_Service", new { bookingId = booking_Service.BookingId, serviceId = booking_Service.ServiceId }, booking_Service);
        }

        /// <summary>
        /// Deletes a booking service by composite key (BookingId, ServiceId).
        /// </summary>
        /// <param name="bookingId">The Booking ID of the booking service to delete.</param>
        /// <param name="serviceId">The Service ID of the booking service to delete.</param>
        /// <example>
        /// DELETE http://localhost:7198/api/Booking_Service/5/10
        /// </example>
        /// <returns>
        /// 204 No Content on successful deletion,
        /// 404 Not Found if the booking service does not exist.
        /// </returns>
        [HttpDelete("{bookingId}/{serviceId}")]
        public async Task<IActionResult> DeleteBooking_Service(int bookingId, int serviceId)
        {
            var booking_Service = await _context.Booking_Services.FindAsync(bookingId, serviceId);
            if (booking_Service == null)
            {
                return NotFound();
            }

            _context.Booking_Services.Remove(booking_Service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Booking_ServiceExists(int bookingId, int serviceId)
        {
            return _context.Booking_Services.Any(e => e.BookingId == bookingId && e.ServiceId == serviceId);
        }


        // GET: api/Booking_Service/ServicesForBooking/5
        [HttpGet("ServicesForBooking/{bookingId}")]
        public async Task<ActionResult<IEnumerable<ServiceDTO>>> ListServicesForBooking(int bookingId)
        {
            var services = await _context.Booking_Services
                .Where(bs => bs.BookingId == bookingId)
                .Include(bs => bs.Service)
                .Select(bs => new ServiceDTO
                {
                    ServiceId = bs.Service.ServiceId,
                    Name = bs.Service.Name,
                    Price = bs.Service.Price
                })
                .ToListAsync();

            if (!services.Any())
                return NotFound($"No services found for booking ID {bookingId}.");

            return Ok(services);
        }

        // GET: api/Booking_Service/BookingsForService/3
        [HttpGet("BookingsForService/{serviceId}")]
        public async Task<ActionResult<IEnumerable<BookingSummaryDTO>>> ListBookingsForService(int serviceId)
        {
            var bookings = await _context.Booking_Services
                .Where(bs => bs.ServiceId == serviceId)
                .Include(bs => bs.Booking)
                    .ThenInclude(b => b.Client)
                .Include(bs => bs.Booking)
                    .ThenInclude(b => b.Photographer)
                .GroupBy(bs => bs.Booking)
                .Select(g => new BookingSummaryDTO
                {
                    BookingId = g.Key.BookingId,
                    BookingDate = g.Key.BookingDate,
                    Location = g.Key.Location != null ? g.Key.Location : string.Empty,
                    ClientName = g.Key.Client != null ? g.Key.Client.Name : "Unknown Client",
                    PhotographerName = g.Key.Photographer != null ? g.Key.Photographer.Name : "Unknown Photographer",
                    ServiceCount = _context.Booking_Services.Count(x => x.BookingId == g.Key.BookingId)
                })
                .ToListAsync();

            if (!bookings.Any())
                return NotFound($"No bookings found for service ID {serviceId}.");

            return Ok(bookings);
        }


    }
}
