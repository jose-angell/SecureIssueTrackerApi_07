namespace SecureIssueTrackerApi_07.Infrastructure.Security
{
    public interface IPasswordHasher<User>
    {
       public string HashPassword(string password);
       public bool VerifyPassword(string password, string hash);    
    }
}
