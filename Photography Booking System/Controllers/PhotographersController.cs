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
    public class PhotographersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PhotographersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of photographers in the system.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Photographers -> [{"PhotographerId":1,"Name":"Kim Smith"}, {"PhotographerId":2,"Name":"Emily Johnson"}, ...]
        /// </example>
        /// <returns>
        /// A list of Photographer objects.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Photographer>>> GetPhotographers()
        {
            return await _context.Photographers.ToListAsync();
        }

        /// <summary>
        /// Returns a single photographer by ID.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Photographers/5 -> {"PhotographerId":5,"Name":"Kim Smith"}
        /// </example>
        /// <returns>
        /// A Photographer object if found; otherwise, 404 Not Found.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Photographer>> GetPhotographer(int id)
        {
            var photographer = await _context.Photographers.FindAsync(id);

            if (photographer == null)
            {
                return NotFound();
            }

            return photographer;
        }

        /// <summary>
        /// Updates an existing photographer in the system.
        /// </summary>
        /// <example>
        /// PUT http://localhost:7198/api/Photographers/5
        /// Body: {"PhotographerId":5,"Name":"Updated Name"}
        /// </example>
        /// <returns>
        /// NoContent on success, BadRequest if ID mismatch, NotFound if photographer does not exist.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhotographer(int id, Photographer photographer)
        {
            if (id != photographer.PhotographerId)
            {
                return BadRequest();
            }

            _context.Entry(photographer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotographerExists(id))
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
        /// Adds a new photographer to the system.
        /// </summary>
        /// <example>
        /// POST http://localhost:7198/api/Photographers
        /// Body: {"Name":"New Photographer"}
        /// </example>
        /// <returns>
        /// The newly created Photographer object with location header.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<Photographer>> PostPhotographer(Photographer photographer)
        {
            _context.Photographers.Add(photographer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhotographer", new { id = photographer.PhotographerId }, photographer);
        }

        /// <summary>
        /// Deletes a photographer by ID.
        /// </summary>
        /// <example>
        /// DELETE http://localhost:7198/api/Photographers/5
        /// </example>
        /// <returns>
        /// NoContent on success, NotFound if photographer does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhotographer(int id)
        {
            var photographer = await _context.Photographers.FindAsync(id);
            if (photographer == null)
            {
                return NotFound();
            }

            _context.Photographers.Remove(photographer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhotographerExists(int id)
        {
            return _context.Photographers.Any(e => e.PhotographerId == id);
        }
    }
}

