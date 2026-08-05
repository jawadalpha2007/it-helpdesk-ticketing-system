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
        private readonly IActivityLogService _activityLogService;

        public TicketService(AppDbContext context, IActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }
        public async Task<List<TicketResponseDto>> GetAllTicketsAsync(int userId, string role)
        {
            var query = _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if (role == "Employee")
            {
                query = query.Where(t => t.CreatedBy == userId);
            }
            else if (role == "IT Support Agent")
            {
                query = query.Where(t => t.AssignedTo == null || t.AssignedTo == userId);
            }
            // Manager and Admin see everything — no filter applied

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
            await _activityLogService.LogActionAsync(
      request.CreatedBy,
      "Ticket Created",
      "Ticket",
      newTicket.Id
  );
            // Reload with related data so the response has readable names
            return await GetTicketByIdAsync(newTicket.Id)
                ?? throw new Exception("Failed to reload created ticket.");
        }
        public async Task<TicketResponseDto?> UpdateTicketAsync(int id, UpdateTicketDto request, string role, int userId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return null;

            var oldStatusId = ticket.StatusId;

            ticket.Title = request.Title;
            ticket.Description = request.Description;
            ticket.CategoryId = request.CategoryId;
            ticket.PriorityId = request.PriorityId;
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

            await _context.SaveChangesAsync();

            await _activityLogService.LogActionAsync(userId, "Ticket Updated", "Ticket", ticket.Id);

            if (oldStatusId != ticket.StatusId)
            {
                var statusName = await _context.Statuses.FirstOrDefaultAsync(s => s.Id == ticket.StatusId);
                await _activityLogService.LogActionAsync(
                    userId,
                    $"Status changed to {statusName?.StatusName ?? "Unknown"}",
                    "Ticket",
                    ticket.Id
                );
            }

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
            double? resolutionHours = null;
            if (ticket.ResolvedAt.HasValue)
            {
                resolutionHours = (ticket.ResolvedAt.Value - ticket.CreatedAt).TotalHours;
            }

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
                ResolvedAt = ticket.ResolvedAt,
                AssignedToId = ticket.AssignedTo,
                ResolutionTimeHours = resolutionHours

            };
        }

        public async Task<TicketResponseDto?> SelfAssignTicketAsync(int ticketId, int userId, string role)
        {
            if (role != "IT Support Agent")
                throw new UnauthorizedAccessException("Only IT Support Agents can self-assign tickets.");

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return null;

            if (ticket.AssignedTo != null)
                throw new InvalidOperationException("This ticket is already assigned.");

            ticket.AssignedTo = userId;
            ticket.StatusId = 2; // In Progress
            ticket.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _activityLogService.LogActionAsync(userId, "Self-assigned ticket", "Ticket", ticket.Id);

            return await GetTicketByIdAsync(ticket.Id);
        }

        public async Task<TicketResponseDto?> AssignTicketAsync(int ticketId, int agentId, int performedByUserId, string performedByRole)
        {
            if (performedByRole != "Manager" && performedByRole != "Admin")
                throw new UnauthorizedAccessException("Only Managers or Admins can assign tickets to agents.");

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return null;

            var agent = await _context.Users.FirstOrDefaultAsync(u => u.Id == agentId);
            if (agent == null || agent.RoleId != 2) // 2 = IT Support Agent
                throw new InvalidOperationException("The selected user is not a valid IT Support Agent.");

            ticket.AssignedTo = agentId;
            ticket.StatusId = 2; // In Progress
            ticket.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _activityLogService.LogActionAsync(
                performedByUserId,
                $"Assigned ticket to {agent.FullName}",
                "Ticket",
                ticket.Id
            );

            return await GetTicketByIdAsync(ticket.Id);
        }



    }
}
