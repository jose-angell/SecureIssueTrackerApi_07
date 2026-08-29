using SecureIssueTrackerApi_07.Domain;
using System.ComponentModel.DataAnnotations;

namespace SecureIssueTrackerApi_07.Dtos.Ticket
{
    public class CreateTicket
    {
        [Required(ErrorMessage = "El titulo del Ticket es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El titulo debe tener como maximo 150 caractires.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "La descripcion del ticket es obligatoria.")]
        [MaxLength(1000, ErrorMessage = "La descripcion debe tener como maximo 1000 caractires.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El nivel de prioridad es obligatorio.")]
        public TicketPriority? Priority { get; set; }

    }
}
