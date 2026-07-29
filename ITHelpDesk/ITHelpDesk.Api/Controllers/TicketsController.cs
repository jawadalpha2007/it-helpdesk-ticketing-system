using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ITHelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var tickets = await _ticketService.GetAllTicketsAsync(userId, role);
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
                return NotFound(new { message = "Ticket not found." });

            return Ok(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto request)
        {
            var createdTicket = await _ticketService.CreateTicketAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdTicket.Id }, createdTicket);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto request)
        {
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var updatedTicket = await _ticketService.UpdateTicketAsync(id, request,role);

            if (updatedTicket == null)
                return NotFound(new { message = "Ticket not found." });

            return Ok(updatedTicket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _ticketService.DeleteTicketAsync(id);

            if (!success)
                return NotFound(new { message = "Ticket not found." });

            return NoContent();
        }

    }
}
