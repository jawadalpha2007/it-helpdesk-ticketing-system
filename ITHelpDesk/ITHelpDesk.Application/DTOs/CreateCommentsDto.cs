using System;
namespace ITHelpDesk.Application.DTOs
{
    public class CreateCommentDto
    {
        public string CommentText { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
    }
}

