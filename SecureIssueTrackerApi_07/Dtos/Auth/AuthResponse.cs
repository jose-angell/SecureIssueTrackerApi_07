using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Dtos.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }
}
