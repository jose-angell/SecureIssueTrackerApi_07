using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Infrastructure.Security
{
    public interface ICurrentUserService
    {
        public Guid GetCurrentUser();
        public UserRole GetCurrentUserRole();
    }
}
