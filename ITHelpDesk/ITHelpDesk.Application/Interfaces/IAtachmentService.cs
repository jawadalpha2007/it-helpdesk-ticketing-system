using ITHelpDesk.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ITHelpDesk.Application.Interfaces
{
    public interface IAttachmentService
    {
        Task<AttachmentResponseDto> UploadAttachmentAsync(int ticketId, int userId, IFormFile file);
        Task<List<AttachmentResponseDto>> GetAttachmentsForTicketAsync(int ticketId);
    }
}