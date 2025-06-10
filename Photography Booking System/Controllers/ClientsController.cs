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
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of clients in the system.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Clients/List -> [{"ClientId":1,"Name":"Lisa Smith"},{"ClientId":2,"Name":"Jackie William"},...]
        /// </example>
        /// <returns>
        /// A list of Client objects.
        /// </returns>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<Client>>> ListClients()
        {
            return await _context.Clients.ToListAsync();
        }

        /// <summary>
        /// Returns a single client by ID.
        /// </summary>
        /// <example>
        /// GET http://localhost:7198/api/Clients/Find/5 -> {"ClientId":5,"Name":"Jackie William"}
        /// </example>
        /// <returns>
        /// A Client object if found; otherwise, 404 Not Found.
        /// </returns>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<Client>> FindClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return client;
        }

        /// <summary>
        /// Adds a new client to the system.
        /// </summary>
        /// <example>
        /// POST http://localhost:7198/api/Clients/Add
        /// Body: {"Name":"Sarah Jackson", "Email":"sarah@google.com"}
        /// </example>
        /// <returns>
        /// The newly created Client object with location header.
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<Client>> AddClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(FindClient), new { id = client.ClientId }, client);
        }

        /// <summary>
        /// Updates an existing client in the system.
        /// </summary>
        /// <example>
        /// PUT http://localhost:7198/api/Clients/Update/5
        /// Body: {"ClientId":5,"Name":"Updated Name"}
        /// </example>
        /// <returns>
        /// NoContent on success, BadRequest if ID mismatch, NotFound if client does not exist.
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateClient(int id, Client client)
        {
            if (id != client.ClientId) return BadRequest();

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a client by ID.
        /// </summary>
        /// <example>
        /// DELETE http://localhost:7198/api/Clients/Delete/5
        /// </example>
        /// <returns>
        /// NoContent on success, NotFound if client does not exist.
        /// </returns>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}
