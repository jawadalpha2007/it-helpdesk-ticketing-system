namespace ITHelpDesk.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public double? AverageResolutionHours { get; set; }

        public List<StatusCountDto> TicketsByStatus { get; set; } = new();
        public List<PriorityCountDto> TicketsByPriority { get; set; } = new();
        public List<CategoryCountDto> TicketsByCategory { get; set; } = new();
    }

    public class StatusCountDto
    {
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class PriorityCountDto
    {
        public string PriorityName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CategoryCountDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}