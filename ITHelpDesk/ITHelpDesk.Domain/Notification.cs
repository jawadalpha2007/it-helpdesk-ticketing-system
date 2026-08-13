using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITHelpDesk.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? TicketId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
        public Ticket? Ticket { get; set; }
    }
}