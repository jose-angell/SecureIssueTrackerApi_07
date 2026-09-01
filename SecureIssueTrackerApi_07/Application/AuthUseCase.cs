using Microsoft.EntityFrameworkCore;
using SecureIssueTrackerApi_07.Domain;
using SecureIssueTrackerApi_07.Dtos.Auth;
using SecureIssueTrackerApi_07.Exceptions;
using SecureIssueTrackerApi_07.Infrastructure;

namespace SecureIssueTrackerApi_07.Application
{
    public class AuthUseCase
    {
        private readonly AppDbContext _context;
        public AuthUseCase(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AuthResponse> Create(RegisterCustomerRequest request)
        {
            var existEmail = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (existEmail) throw new ConflictException("El correo no esta disponible.");

            // crear password hash
            var passwordHash = request.Password!;

            var role = UserRole.Customer;

            var newUser = new User(request.FullName!, request.Email!, passwordHash, role);
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return new AuthResponse
            {
                AccessToken = "sample_access_token", // This should be generated properly
                UserId = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Role = newUser.Role,
            };
        }
    }
}
