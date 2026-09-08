using Microsoft.EntityFrameworkCore;
using SecureIssueTrackerApi_07.Application.Security;
using SecureIssueTrackerApi_07.Domain;
using SecureIssueTrackerApi_07.Dtos.Auth;
using SecureIssueTrackerApi_07.Exceptions;
using SecureIssueTrackerApi_07.Infrastructure;

namespace SecureIssueTrackerApi_07.Application
{
    public class AuthUseCase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthUseCase(AppDbContext context, IPasswordHashService passwordHashService, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHashService = passwordHashService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<AuthResponse> Create(RegisterCustomerRequest request)
        {
            var existEmail = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (existEmail) throw new ConflictException("El correo no esta disponible.");

            // crear password hash
            var passwordHash = _passwordHashService.Hash(request.Password!);

            var role = UserRole.Customer;

            var newUser = new User(request.FullName!, request.Email!, passwordHash, role);
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            var token = _jwtTokenGenerator.Generate(newUser);

            return new AuthResponse
            {
                AccessToken = token,
                UserId = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Role = newUser.Role
            };
        }
        public async Task<AuthResponse> Login(LoginCustomerRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == request.Email);

            if (user is null)
                throw new UnauthorizedException("Credenciales inválidas.");

            if (!user.IsActive)
                throw new ForbiddenException("El usuario está inactivo.");

            var isValidPassword = _passwordHashService.Verify(
                request.Password!,
                user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedException("Credenciales inválidas.");

            var token = _jwtTokenGenerator.Generate(user);

            return new AuthResponse
            {
                AccessToken = token,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
