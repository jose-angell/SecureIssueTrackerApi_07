using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Application.Security
{
    public interface ICurrentUserService
    {
        bool IsAuthenticated { get; }
        Guid UserId { get; }
        string? Email { get; }
        UserRole Role { get; }
    }
}
