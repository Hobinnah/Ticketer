// ===================================THIS FILE WAS AUTO GENERATED===================================

using Ticketer.Api.DTOs;
using Ticketer.Api.Models;
using Ticketer.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ticketer.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        // GET: api/<TicketController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [HttpGet(Name = "GetAllTickets")]
        public async Task<IActionResult> Get([FromServices] ITicketService ticketService, [FromQuery] Paging paging)
        {
            return Ok(await ticketService.GetAll(paging));
        }

        // GET api/<TicketController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [HttpGet("{id}", Name = "GetTicketByID")]
        public async Task<IActionResult> GetTicketByID([FromServices] ITicketService ticketService, int id)
        {
            return Ok(await ticketService.GetID(id));
        }

        // POST api/<TicketController>
        [Authorize(Roles = "User, Admin")]
        [HttpPost(Name = "CreateTicket")]
        public async Task<IActionResult> CreateTicket([FromServices] ITicketService ticketService, [FromBody] TicketDto ticketDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await ticketService.CreateTicket(ticketDto));
        }

        // PUT api/<TicketController>/5
        [Authorize(Roles = "User, Admin")]
        [HttpPut("{id}", Name = "UpdateTicket")]
        public async Task<IActionResult> UpdateTicket([FromServices] ITicketService ticketService, int id, [FromBody] TicketDto ticketDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await ticketService.UpdateTicket(id, ticketDto));
        }

        // DELETE api/<TicketController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteTicket")]
        public async Task<IActionResult> DeleteTicket([FromServices] ITicketService ticketService, int id)
        {
            await ticketService.DeleteTicket(id);

            return Ok();
        }

        // Get: api/ticket/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpGet("{id}/{status}", Name = "UpdateTicketStatus")]
        public async Task<ActionResult<Task>> UpdateTicketStatus([FromServices] ITicketService ticketService, [FromRoute] int id, [FromRoute] string status)
        {
            var ticket = await ticketService.UpdateTicketStatus(id, status);
            if (ticket == null)
                return NotFound($"Ticket with ID {id} not found.");

            return Ok(ticket);
        }
    }
}
