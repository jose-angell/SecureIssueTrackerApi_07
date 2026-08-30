using SecureIssueTrackerApi_07.Exceptions;

namespace SecureIssueTrackerApi_07.Domain
{
    public class Ticket
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public TicketStatus Status { get; private set; }
        public TicketPriority Priority { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public Guid? AssignedToUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        public User CreatedByUser { get; private set; } = null!;
        public User? AssignedToUser { get; private set; }

        private Ticket() { }

        public Ticket(string title, string description, TicketPriority priority, Guid createdByUserId)
        {
            Validate(title, description, priority, createdByUserId);
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Status = TicketStatus.Open;
            Priority = priority;
            CreatedByUserId = createdByUserId;
            CreatedAt = DateTime.UtcNow;
        }
        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) throw new DomainException("La descripcion no puede ser null o estar vacia.");
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Open()
        {
            if (Status == TicketStatus.Closed) throw new DomainException("Un ticket cerrado no puede reabrirse en esta version.");
            Status = TicketStatus.Open;
            UpdatedAt = DateTime.UtcNow;
        }
        public void InProgress()
        {
            if (Status != TicketStatus.Open) throw new DomainException("Solo un ticket abierto puede pasar a estar en progreso.");
            Status = TicketStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Resolved()
        {
            if (Status != TicketStatus.InProgress) throw new DomainException("Solo un ticket en progreso puede ser resulto.");
            Status = TicketStatus.Resolved;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Closed()
        {
            if (Status != TicketStatus.Resolved) throw new DomainException("Solo un ticket resuelto puede ser cerado.");
            Status = TicketStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }
        public void AssignTo(Guid agentId)
        {
            if (agentId == Guid.Empty) throw new DomainException("El Id del agente asignado no puede estar vacio.");
            if (Status == TicketStatus.Closed) throw new DomainException("Un ticket cerrado no puede ser asignado.");
            AssignedToUserId = agentId;
            UpdatedAt = DateTime.UtcNow;
        }
        private void Validate(string title, string description, TicketPriority priority, Guid createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("El titulo no puede ser null o estar vacio.");
            if (string.IsNullOrWhiteSpace(description)) throw new DomainException("La descripcion no puede ser null o estar vacia.");
            if (!Enum.IsDefined(typeof(TicketPriority), priority)) throw new DomainException("La prioridad no es valida.");
            if (createdByUserId == Guid.Empty) throw new DomainException("El Id de usuario de creacion no puede estar vacio.");
        }
    }
}
