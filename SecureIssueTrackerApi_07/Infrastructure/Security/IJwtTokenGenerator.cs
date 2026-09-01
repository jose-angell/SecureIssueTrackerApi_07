using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(User user);
    }
}
