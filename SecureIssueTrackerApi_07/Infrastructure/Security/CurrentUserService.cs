using SecureIssueTrackerApi_07.Application.Security;
using SecureIssueTrackerApi_07.Domain;
using System.Security.Claims;

namespace SecureIssueTrackerApi_07.Infrastructure.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(value, out var userId))
                    throw new UnauthorizedAccessException("Usuario no autenticado.");

                return userId;
            }
        }

        public string? Email =>
            _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.Email);

        public UserRole Role
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.Role);

                if (!Enum.TryParse<UserRole>(value, out var role))
                    throw new UnauthorizedAccessException("Rol de usuario no válido.");

                return role;
            }
        }

    }
}
