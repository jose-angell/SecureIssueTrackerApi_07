using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SecureIssueTrackerApi_07.Application;
using SecureIssueTrackerApi_07.Dtos.Auth;

namespace SecureIssueTrackerApi_07.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly AuthUseCase _useCase;
        public AuthController(AuthUseCase useCase)
        {
            _useCase = useCase;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerRequest request)
        {
            var response = await _useCase.RegisterCustomer(request);
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCustomerRequest request)
        {
            var response = await _useCase.Login(request);
            return Ok(response);
        }
    }
}
