using SecureIssueTrackerApi_07.Exceptions;

namespace SecureIssueTrackerApi_07.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public ICollection<Ticket> Tickets { get; } = new List<Ticket>();
        private User() { }

        public User(string fullName, string email, string passwordHash, UserRole role)
        {
            Validate(fullName,email,passwordHash,role);
            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            IsActive = true;
            CreatedAt = DateTime.Now;
        }
        public void Activate()
        {
            IsActive = true;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
        private void Validate(string fullName, string email, string passwordHash, UserRole role)
        {
            if (String.IsNullOrWhiteSpace(fullName)) throw new DomainException("El nombre no puede ser null o estar vacio.");
            if (String.IsNullOrWhiteSpace(email)) throw new DomainException("El correo no puede ser null o estar vacio.");
            if (String.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("La contraseña no puede ser null o estar vacia.");
            if (!Enum.IsDefined(typeof(UserRole), role)) throw new DomainException("El role no es valido.");
        }
    }
}
