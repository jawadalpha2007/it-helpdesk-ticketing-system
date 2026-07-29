using ITHelpDesk.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Application.Interfaces
{
    public interface ITicketService
    {
        Task<List<TicketResponseDto>> GetAllTicketsAsync(int userId, string role);
        Task<TicketResponseDto?> GetTicketByIdAsync(int id);
        Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto request);
        Task<TicketResponseDto?> UpdateTicketAsync(int id, UpdateTicketDto request,string role );
        Task<bool> DeleteTicketAsync(int id);
    }
}
