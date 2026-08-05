using System;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITHelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId}/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var comments = await _commentService.GetCommentsForTicketAsync(ticketId, role);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, [FromBody] CreateCommentDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var comment = await _commentService.AddCommentAsync(ticketId, userId, role, request);
            return Ok(comment);
        }
    }
}
