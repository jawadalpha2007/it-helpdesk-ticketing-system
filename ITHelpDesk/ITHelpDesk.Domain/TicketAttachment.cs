using System;
namespace ITHelpDesk.Domain.Entities
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UploadedBy { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public Ticket? Ticket { get; set; }
        public User? UploadedByUser { get; set; }
    }
}