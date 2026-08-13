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
    var role = User.FindFirst(ClaimTypes.Role)!.Value;

    if (role != "Employee" && role != "Admin")
        return Forbid();

    var createdTicket = await _ticketService.CreateTicketAsync(request);
    return CreatedAtAction(nameof(GetById), new { id = createdTicket.Id }, createdTicket);
}
      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var updatedTicket = await _ticketService.UpdateTicketAsync(id, request, role, userId);

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
        [HttpGet("{id}/logs")]
        public async Task<IActionResult> GetLogs(int id, [FromServices] IActivityLogService activityLogService)
        {
            var logs = await activityLogService.GetLogsForEntityAsync("Ticket", id);
            return Ok(logs);
        }
        [HttpPost("{id}/self-assign")]
        public async Task<IActionResult> SelfAssign(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            try
            {
                var ticket = await _ticketService.SelfAssignTicketAsync(id, userId, role);
                if (ticket == null)
                    return NotFound(new { message = "Ticket not found." });

                return Ok(ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] AssignTicketDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            try
            {
                var ticket = await _ticketService.AssignTicketAsync(id, request.AgentId, userId, role);
                if (ticket == null)
                    return NotFound(new { message = "Ticket not found." });

                return Ok(ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id}/attachments")]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file, [FromServices] IAttachmentService attachmentService)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var attachment = await attachmentService.UploadAttachmentAsync(id, userId, file);
                return Ok(attachment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/attachments")]
        public async Task<IActionResult> GetAttachments(int id, [FromServices] IAttachmentService attachmentService)
        {
            var attachments = await attachmentService.GetAttachmentsForTicketAsync(id);
            return Ok(attachments);
        }

    }
}
