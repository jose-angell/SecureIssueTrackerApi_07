using Microsoft.AspNetCore.Mvc;
using SecureIssueTrackerApi_07.Application;
using SecureIssueTrackerApi_07.Dtos.Ticket;
using System.Formats.Asn1;

namespace SecureIssueTrackerApi_07.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly TicketUseCase _useCase;
        public TicketController(TicketUseCase useCase)
        {
            _useCase = useCase;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TicketQuery query)
        {
            var result = await _useCase.GetAll(query );
            return Ok(result);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            var result = await _useCase.GetById(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            var result = await _useCase.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [HttpPatch("{id:guid}/description")]
        public async Task<IActionResult> UpdateDescription([FromRoute] Guid id, [FromBody] string description)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            if (String.IsNullOrWhiteSpace(description)) return BadRequest("La descripcion es requerida.");
            await _useCase.UpdateDescription(id, description);
            return NoContent();
        }
        [HttpPatch("{id:guid}/assign")]
        public async Task<IActionResult> AssignTo([FromRoute] Guid id, [FromBody] Guid userId)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            if (userId == Guid.Empty) return BadRequest("El id del usuario asignado es invalido");
            await _useCase.AssignTo(id, userId);
            return NoContent();
        }
        [HttpPatch("{id:guid}/start-progress")]
        public async Task<IActionResult> StartProgress([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            await _useCase.StartProgress(id);
            return NoContent();
        }
        [HttpPatch("{id:guid}/resolve")]
        public async Task<IActionResult> Resolve([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            await _useCase.Resolved(id);
            return NoContent();
        }
        [HttpPatch("{id:guid}/close")]
        public async Task<IActionResult> Close([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            await _useCase.Closed(id);
            return NoContent();
        }
    }
}
