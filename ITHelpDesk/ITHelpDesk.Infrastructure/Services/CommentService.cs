using System;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommentResponseDto>> GetCommentsForTicketAsync(int ticketId, string role)
        {
            var query = _context.TicketComments
                .Include(c => c.User)
                .Where(c => c.TicketId == ticketId)
                .AsQueryable();

            // Employees never see internal notes
            if (role == "Employee")
            {
                query = query.Where(c => c.IsInternal == false);
            }

            var comments = await query
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                CommentText = c.CommentText,
                IsInternal = c.IsInternal,
                AuthorName = c.User?.FullName ?? string.Empty,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<CommentResponseDto> AddCommentAsync(int ticketId, int userId, string role, CreateCommentDto request)
        {
            // Employees can never post internal notes, regardless of what they send
            bool isInternal = role == "Employee" ? false : request.IsInternal;

            var comment = new TicketComment
            {
                TicketId = ticketId,
                UserId = userId,
                CommentText = request.CommentText,
                IsInternal = isInternal,
                CreatedAt = DateTime.Now
            };

            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();

            // Reload with the User included so we can return the author's name
            var savedComment = await _context.TicketComments
                .Include(c => c.User)
                .FirstAsync(c => c.Id == comment.Id);

            return new CommentResponseDto
            {
                Id = savedComment.Id,
                CommentText = savedComment.CommentText,
                IsInternal = savedComment.IsInternal,
                AuthorName = savedComment.User?.FullName ?? string.Empty,
                CreatedAt = savedComment.CreatedAt
            };
        }
    }
}