using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.Models;

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
        /// Returns a specific booking service by BookingId.
        /// </summary>
        /// <param name="id">The ID of the booking service to retrieve.</param>
        /// <example>
        /// GET http://localhost:7198/api/Booking_Service/5
        /// -> {"BookingId":5, "ServiceName":"Birthday Shoot", ...}
        /// </example>
        /// <returns>A Booking_Service object if found; otherwise, 404 Not Found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking_Service>> GetBooking_Service(int id)
        {
            var booking_Service = await _context.Booking_Services.FindAsync(id);

            if (booking_Service == null)
            {
                return NotFound();
            }

            return booking_Service;
        }

        /// <summary>
        /// Updates an existing booking service identified by BookingId.
        /// </summary>
        /// <param name="id">The ID of the booking service to update.</param>
        /// <param name="booking_Service">The updated booking service object.</param>
        /// <example>
        /// PUT http://localhost:7198/api/Booking_Service/5
        /// Body: {"BookingId":5, "ServiceName":"Updated Service", ...}
        /// </example>
        /// <returns>
        /// 204 No Content on successful update,
        /// 400 Bad Request if ID in URL and body do not match,
        /// 404 Not Found if the booking service does not exist.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking_Service(int id, Booking_Service booking_Service)
        {
            if (id != booking_Service.BookingId)
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
                if (!Booking_ServiceExists(id))
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
        /// Body: {"BookingId":10, "ServiceName":"New Service", ...}
        /// </example>
        /// <returns>
        /// 201 Created with the created booking service object,
        /// 409 Conflict if a booking service with the same ID already exists.
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
                if (Booking_ServiceExists(booking_Service.BookingId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBooking_Service", new { id = booking_Service.BookingId }, booking_Service);
        }

        /// <summary>
        /// Deletes a booking service by BookingId.
        /// </summary>
        /// <param name="id">The ID of the booking service to delete.</param>
        /// <example>
        /// DELETE http://localhost:7198/api/Booking_Service/5
        /// </example>
        /// <returns>
        /// 204 No Content on successful deletion,
        /// 404 Not Found if the booking service does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking_Service(int id)
        {
            var booking_Service = await _context.Booking_Services.FindAsync(id);
            if (booking_Service == null)
            {
                return NotFound();
            }

            _context.Booking_Services.Remove(booking_Service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Booking_ServiceExists(int id)
        {
            return _context.Booking_Services.Any(e => e.BookingId == id);
        }
    }
}
