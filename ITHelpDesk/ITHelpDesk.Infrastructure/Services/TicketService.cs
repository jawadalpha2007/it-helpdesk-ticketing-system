using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;

        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketResponseDto>> GetAllTicketsAsync(int userId,string role)
        
        {
            var query = _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            // Employees only see tickets they personally created
            if (role == "Employee")
            {
                query = query.Where(t => t.CreatedBy == userId);
            }

            var tickets = await query.ToListAsync();

            return tickets.Select(MapToDto).ToList();
        }
        

        public async Task<TicketResponseDto?> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return null;

            return MapToDto(ticket);
        }

        public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto request)
        {
            var newTicket = new Ticket
            {
                TicketReference = GenerateTicketReference(),
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId,
                PriorityId = request.PriorityId,
                StatusId = 1, // 1 = "Open" based on your seed data order
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();

            // Reload with related data so the response has readable names
            return await GetTicketByIdAsync(newTicket.Id)
                ?? throw new Exception("Failed to reload created ticket.");
        }

        public async Task<TicketResponseDto?> UpdateTicketAsync(int id, UpdateTicketDto request,string role )
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return null;

            ticket.Title = request.Title;
            ticket.Description = request.Description;
            ticket.CategoryId = request.CategoryId;
            ticket.PriorityId = request.PriorityId;
            ticket.StatusId = request.StatusId;
            ticket.AssignedTo = request.AssignedTo;
            ticket.UpdatedAt = DateTime.Now;

            if (role == "Admin" || role == "IT Support Agent" || role == "Manager")
            {
                ticket.StatusId = request.StatusId;
                ticket.AssignedTo = request.AssignedTo;

                if (request.StatusId == 4 && ticket.ResolvedAt == null)
                {
                    ticket.ResolvedAt = DateTime.Now;
                }
            }

            // If status changed to "Resolved" (assume id 4 based on seed order), stamp ResolvedAt
            if (request.StatusId == 4 && ticket.ResolvedAt == null)
            {
                ticket.ResolvedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticket.Id);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return false;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return true;
        }

        // --- Helper methods ---

        private string GenerateTicketReference()
        {
            var year = DateTime.Now.Year;
            var randomNumber = new Random().Next(1000, 9999);
            return $"TCK-{year}-{randomNumber}";
        }

        private TicketResponseDto MapToDto(Ticket ticket)
        {
            return new TicketResponseDto
            {
                Id = ticket.Id,
                TicketReference = ticket.TicketReference,
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryName = ticket.Category?.CategoryName ?? string.Empty,
                PriorityName = ticket.Priority?.PriorityName ?? string.Empty,
                StatusName = ticket.Status?.StatusName ?? string.Empty,
                CreatedByName = ticket.CreatedByUser?.FullName ?? string.Empty,
                AssignedToName = ticket.AssignedToUser?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ResolvedAt = ticket.ResolvedAt
            };
        }        



    }
}
