using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Application.DTOs
{
    public class TicketResponseDto
    {
        public int Id { get; set; }
        public string TicketReference { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
        public string PriorityName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;

        public string CreatedByName { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
