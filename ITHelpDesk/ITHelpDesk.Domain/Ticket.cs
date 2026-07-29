using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TicketReference { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Category? Category { get; set; }
        public Priority? Priority { get; set; }
        public Status? Status { get; set; }
        public User? CreatedByUser { get; set; }
        public User? AssignedToUser { get; set; }

    }
}
