using System;
namespace ITHelpDesk.Application.DTOs
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}