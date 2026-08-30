using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Dtos.Ticket
{
    public class TicketQuery
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TicketStatus? Status { get; set; }
        public TicketPriority? Priority { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
