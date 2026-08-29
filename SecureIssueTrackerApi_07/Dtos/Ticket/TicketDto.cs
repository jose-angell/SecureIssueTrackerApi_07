using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Dtos.Ticket
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }

        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}
