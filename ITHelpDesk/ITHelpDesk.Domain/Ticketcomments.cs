using System;
namespace ITHelpDesk.Domain.Entities
{
    public class TicketComment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Ticket? Ticket { get; set; }
        public User? User { get; set; }
    }
}