using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Application.Security
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user);
    }
}
