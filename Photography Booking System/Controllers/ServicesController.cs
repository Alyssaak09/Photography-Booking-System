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
    public class ServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of services in the system.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Services/List -> [{"ServiceId":1,"Name":"Wedding Photography","Price":500.0},{"ServiceId":2,"Name":"Portrait Session","Price":150.0}]
        /// </example>
        /// <returns>
        /// A list of ServiceDTO objects.
        /// </returns>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ServiceDTO>>> ListServices()
        {
            var services = await _context.Services.ToListAsync();

            var serviceDtos = services.Select(s => new ServiceDTO
            {
                ServiceId = s.ServiceId,
                Name = s.Name,
                Price = s.Price
            }).ToList();

            return Ok(serviceDtos);
        }

        /// <summary>
        /// Returns a single service by ID.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Services/Find/2 -> {"ServiceId":2,"Name":"Portrait Session","Price":150.0}
        /// </example>
        /// <returns>
        /// A ServiceDTO object if found; otherwise, 404 Not Found.
        /// </returns>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ServiceDTO>> FindService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            var dto = new ServiceDTO
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Price = service.Price
            };

            return Ok(dto);
        }

        /// <summary>
        /// Adds a new service to the system.
        /// </summary>
        /// <example>
        /// POST http://localhost:7198/api/Services/Add
        /// Body: {"Name":"Event Photography","Price":300.0}
        /// </example>
        /// <returns>
        /// The newly created ServiceDTO object with location header.
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<ServiceDTO>> AddService(ServiceDTO serviceDto)
        {
            var service = new Service
            {
                Name = serviceDto.Name,
                Price = serviceDto.Price
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            serviceDto.ServiceId = service.ServiceId;

            return CreatedAtAction(nameof(FindService), new { id = service.ServiceId }, serviceDto);
        }

        /// <summary>
        /// Updates an existing service in the system.
        /// </summary>
        /// <example>
        /// PUT http://localhost:7198/api/Services/Update/3
        /// Body: {"ServiceId":3,"Name":"Updated Service","Price":275.0}
        /// </example>
        /// <returns>
        /// NoContent on success, BadRequest if ID mismatch, NotFound if service does not exist.
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateService(int id, ServiceDTO serviceDto)
        {
            if (id != serviceDto.ServiceId) return BadRequest();

            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.Name = serviceDto.Name;
            service.Price = serviceDto.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a service by ID.
        /// </summary>
        /// <example>
        /// DELETE http://localhost:7198/api/Services/Delete/3
        /// </example>
        /// <returns>
        /// NoContent on success, NotFound if service does not exist.
        /// </returns>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Lists bookings associated with a specific service.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Services/ListBookingsByService/2
        /// </example>
        /// <param name="serviceId">ID of the service.</param>
        /// <returns>
        /// A list of BookingSummaryDTO objects for bookings related to the specified service.
        /// </returns>
        [HttpGet("ListBookingsByService/{serviceId}")]
        public async Task<ActionResult<IEnumerable<BookingSummaryDTO>>> ListBookingsByService(int serviceId)
        {
            var bookings = await _context.Booking_Services
                .Where(bs => bs.ServiceId == serviceId
                             && bs.Booking != null
                             && bs.Booking.Client != null
                             && bs.Booking.Photographer != null)
                .Include(bs => bs.Booking)
                    .ThenInclude(b => b.Client)
                .Include(bs => bs.Booking)
                    .ThenInclude(b => b.Photographer)
               
                .Select(bs => new BookingSummaryDTO
                {
                    BookingId = bs.Booking.BookingId,
                    BookingDate = bs.Booking.BookingDate,
                    Location = bs.Booking.Location ?? string.Empty,
                    ClientName = bs.Booking.Client.Name,
                    PhotographerName = bs.Booking.Photographer.Name,
                    ServiceCount = bs.Booking.BookingServices.Count() 
                })
                .ToListAsync();

            return Ok(bookings);
        }


        /// <summary>
        /// Checks whether a service exists by ID.
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <returns>True if the service exists, false otherwise.</returns>
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}

