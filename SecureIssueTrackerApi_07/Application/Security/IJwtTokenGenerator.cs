using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Application.Security
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(User user);
    }
}
