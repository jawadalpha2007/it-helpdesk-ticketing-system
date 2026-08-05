using System;

namespace ITHelpDesk.Domain.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }
}