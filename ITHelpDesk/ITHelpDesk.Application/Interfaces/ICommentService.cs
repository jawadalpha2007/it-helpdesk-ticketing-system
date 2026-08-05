using ITHelpDesk.Application.DTOs;

namespace ITHelpDesk.Application.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentResponseDto>> GetCommentsForTicketAsync(int ticketId, string role);
        Task<CommentResponseDto> AddCommentAsync(int ticketId, int userId, string role, CreateCommentDto request);
    }
}