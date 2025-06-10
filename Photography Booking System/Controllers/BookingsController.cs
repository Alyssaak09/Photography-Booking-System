using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.Models;
using Photography_Booking_System.DTOs;

namespace Photography_Booking_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of bookings in the system.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Bookings/List -> [{"BookingId":1,"ClientName":"Lisa","PhotographerName":"Jim","Services":[{"ServiceId":1,"Name":"Wedding"}]}, ...]
        /// </example>
        /// <returns>
        /// A list of BookingDTO objects with related client, photographer, and service information.
        /// </returns>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> ListBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Photographer)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new BookingDTO
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                Location = b.Location,
                ClientId = b.ClientId,
                ClientName = b.Client.Name,
                PhotographerId = b.PhotographerId,
                PhotographerName = b.Photographer.Name,
                Services = b.BookingServices.Select(bs => new ServiceDTO
                {
                    ServiceId = bs.ServiceId,
                    Name = bs.Service.Name,
                    Price = bs.Service.Price
                }).ToList()
            }).ToList();

            return Ok(bookingDtos);
        }

        /// <summary>
        /// Returns a booking specified by ID.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Bookings/Find/5 -> {"BookingId":5,"ClientName":"Lisa","PhotographerName":"Jim","Services":[{"ServiceId":1,"Name":"Wedding"}]}
        /// </example>
        /// <returns>
        /// A single BookingDTO object if found; otherwise, NotFound.
        /// </returns>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<BookingDTO>> FindBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Photographer)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            var bookingDto = new BookingDTO
            {
                BookingId = booking.BookingId,
                BookingDate = booking.BookingDate,
                Location = booking.Location,
                ClientId = booking.ClientId,
                ClientName = booking.Client.Name,
                PhotographerId = booking.PhotographerId,
                PhotographerName = booking.Photographer.Name,
                Services = booking.BookingServices.Select(bs => new ServiceDTO
                {
                    ServiceId = bs.ServiceId,
                    Name = bs.Service.Name,
                    Price = bs.Service.Price
                }).ToList()
            };

            return Ok(bookingDto);
        }

        /// <summary>
        /// Updates an existing booking in the system.
        /// </summary>
        /// <example>
        /// PUT http://localhost:7198/api/Bookings/Update/5
        /// </example>
        /// <returns>
        /// Returns NoContent on success, BadRequest if IDs mismatch, or NotFound if booking or related entities do not exist.
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, BookingDTO bookingDto)
        {
            if (id != bookingDto.BookingId)
                return BadRequest();

            var client = await _context.Clients.FindAsync(bookingDto.ClientId);
            var photographer = await _context.Photographers.FindAsync(bookingDto.PhotographerId);

            if (client == null || photographer == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            // Update booking fields
            booking.BookingDate = bookingDto.BookingDate;
            booking.Location = bookingDto.Location;
            booking.ClientId = bookingDto.ClientId;
            booking.PhotographerId = bookingDto.PhotographerId;

            // Update services
            _context.Booking_Services.RemoveRange(booking.BookingServices);

            booking.BookingServices = bookingDto.Services.Select(s => new Booking_Service
            {
                BookingId = id,
                ServiceId = s.ServiceId
            }).ToList();

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Adds a new booking to the system.
        /// </summary>
        /// <example>
        /// POST http://localhost:7198/api/Bookings/Add
        /// </example>
        /// <returns>
        /// Returns Created with the new booking if successful, or NotFound if related entities do not exist.
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<Booking>> AddBooking(BookingDTO bookingDto)
        {
            var client = await _context.Clients.FindAsync(bookingDto.ClientId);
            var photographer = await _context.Photographers.FindAsync(bookingDto.PhotographerId);

            if (client == null || photographer == null)
                return NotFound();

            var booking = new Booking
            {
                BookingDate = bookingDto.BookingDate,
                Location = bookingDto.Location,
                ClientId = bookingDto.ClientId,
                PhotographerId = bookingDto.PhotographerId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            if (bookingDto.Services != null)
            {
                foreach (var serviceDto in bookingDto.Services)
                {
                    _context.Booking_Services.Add(new Booking_Service
                    {
                        BookingId = booking.BookingId,
                        ServiceId = serviceDto.ServiceId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("FindBooking", new { id = booking.BookingId }, bookingDto);
        }

        /// <summary>
        /// Deletes a booking from the system.
        /// </summary>
        /// <example>
        /// DELETE http://localhost:7198/api/Bookings/Delete/5
        /// </example>
        /// <returns>
        /// Returns NoContent on success or NotFound if booking does not exist.
        /// </returns>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            _context.Booking_Services.RemoveRange(booking.BookingServices);

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(b => b.BookingId == id);
        }
    }
}
