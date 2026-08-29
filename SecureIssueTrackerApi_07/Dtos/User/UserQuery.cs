using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Dtos.User
{
    public class UserQuery
    {
        public string? name { get; set; }
        public string? email { get; set; }
        public UserRole? role { get; set; }
        public bool? isActive { get; set; }
    }
}
