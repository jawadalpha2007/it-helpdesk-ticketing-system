using System;
namespace ITHelpDesk.Application.DTOs
{
    public class AttachmentResponseDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}