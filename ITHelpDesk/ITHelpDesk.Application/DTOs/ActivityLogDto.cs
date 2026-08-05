using System;

namespace ITHelpDesk.Application.DTOs
{
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string PerformedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}