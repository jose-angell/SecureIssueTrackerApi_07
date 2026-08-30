using Microsoft.AspNetCore.Mvc;
using SecureIssueTrackerApi_07.Application;
using SecureIssueTrackerApi_07.Dtos.User;

namespace SecureIssueTrackerApi_07.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserUseCase _useCase;
        public UsersController(UserUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            var user = await _useCase.GetById(id);
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var result = await _useCase.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            await _useCase.Update(id, request);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            await _useCase.Delete(id);
            return NoContent();
        }
        [HttpPatch("{id:guid}/activate")]
        public async Task<IActionResult> Activate([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            await _useCase.Activate(id);
            return NoContent();
        }
        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> deactivate([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id es invalido");
            await _useCase.Deactivate(id);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQuery query)
        {
            var user = await _useCase.GetAll(query);
            return Ok(user);
        }
    }
}
