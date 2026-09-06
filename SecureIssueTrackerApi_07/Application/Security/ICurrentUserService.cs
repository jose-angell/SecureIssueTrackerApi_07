using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Application.Security
{
    public interface ICurrentUserService
    {
        public Guid GetCurrentUser();
        public UserRole GetCurrentUserRole();
    }
}
