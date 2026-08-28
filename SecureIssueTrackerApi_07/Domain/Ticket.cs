using SecureIssueTrackerApi_07.Exceptions;
using System.Data;

namespace SecureIssueTrackerApi_07.Domain
{
    public class Ticket
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public TicketStatus Status { get; private set; }
        public TicketPriority PriorityId { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public Guid AssignedToUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        public User CreatedByUser { get; private set; } = null!;
        public User AssignedToUser { get; private set; } = null!;

        private Ticket() { }

        public Ticket(string title, string desciption, TicketPriority priorityId, Guid createdByUserId)
        {
            Validate(title, desciption, priorityId, createdByUserId);
            Id = Guid.NewGuid();
            Title = title;
            Description = desciption;
            Status = TicketStatus.Open;
            PriorityId = priorityId;
            CreatedByUserId = createdByUserId;
            CreatedAt = DateTime.UtcNow;
        }
        public void UpdateDescripcion(string desciption)
        {
            if (string.IsNullOrWhiteSpace(desciption)) throw new DomainException("La descripcion no puede ser null o estar vacia.");
            Description = desciption;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Open()
        {
            if (Status == TicketStatus.Closed) throw new DomainException("Un ticket cerrado no puede reabrirse en esta version.");
            Status = TicketStatus.InProgress;
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
        private void Validate(string title, string desciption, TicketPriority priorityId, Guid createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("El titulo no puede ser null o estar vacio.");
            if (string.IsNullOrWhiteSpace(desciption)) throw new DomainException("La descripcion no puede ser null o estar vacia.");
            if (!Enum.IsDefined(typeof(TicketPriority), priorityId)) throw new DomainException("La prioridad no es valida.");
            if (createdByUserId == Guid.Empty) throw new DomainException("El Id de usuario de creacion no puede estar vacio.");
        }
    }
}
