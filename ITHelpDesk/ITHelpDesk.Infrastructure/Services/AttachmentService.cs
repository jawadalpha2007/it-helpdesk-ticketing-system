using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly AppDbContext _context;
        private readonly string _uploadsFolder;

        public AttachmentService(AppDbContext context)
        {
            _context = context;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<AttachmentResponseDto> UploadAttachmentAsync(int ticketId, int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file was provided.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".txt" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("File type not allowed.");

            const int maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
                throw new InvalidOperationException("File exceeds the 5MB size limit.");

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(_uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new TicketAttachment
            {
                TicketId = ticketId,
                UploadedBy = userId,
                FileName = file.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                FileSize = (int)file.Length,
                FileType = extension,
                UploadedAt = DateTime.Now
            };

            _context.TicketAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            var savedAttachment = await _context.TicketAttachments
                .Include(a => a.UploadedByUser)
                .FirstAsync(a => a.Id == attachment.Id);

            return new AttachmentResponseDto
            {
                Id = savedAttachment.Id,
                FileName = savedAttachment.FileName,
                FilePath = savedAttachment.FilePath,
                FileSize = savedAttachment.FileSize,
                FileType = savedAttachment.FileType,
                UploadedByName = savedAttachment.UploadedByUser?.FullName ?? string.Empty,
                UploadedAt = savedAttachment.UploadedAt
            };
        }

        public async Task<List<AttachmentResponseDto>> GetAttachmentsForTicketAsync(int ticketId)
        {
            var attachments = await _context.TicketAttachments
                .Include(a => a.UploadedByUser)
                .Where(a => a.TicketId == ticketId)
                .OrderBy(a => a.UploadedAt)
                .ToListAsync();

            return attachments.Select(a => new AttachmentResponseDto
            {
                Id = a.Id,
                FileName = a.FileName,
                FilePath = a.FilePath,
                FileSize = a.FileSize,
                FileType = a.FileType,
                UploadedByName = a.UploadedByUser?.FullName ?? string.Empty,
                UploadedAt = a.UploadedAt
            }).ToList();
        }
    }
}
