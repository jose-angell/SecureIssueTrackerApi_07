using Microsoft.EntityFrameworkCore;
using SecureIssueTrackerApi_07.Dtos.User;
using SecureIssueTrackerApi_07.Exceptions;
using SecureIssueTrackerApi_07.Infrastructure;
using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Application
{
    public class UserUseCase
    {
        private readonly AppDbContext _context;
        public UserUseCase(AppDbContext context)
        {
            _context = context;
        }
        public async Task<UserDto> Create( CreateUserRequest request)
        {
            var existEmail = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (existEmail) throw new ConflictException("El correo no esta disponible.");

            // crear password hash
             var passwordHash = request.Password!;

            var newUser = new User(request.FullName!, request.Email!, passwordHash, request.Role!.Value);
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Id = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Role = newUser.Role,
                IsActive = newUser.IsActive,
                CreatedAt = newUser.CreatedAt,
            };
        }
        public async Task Update(Guid id, UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("El usuario no esta en el sistema.");

            var existEmail = await _context.Users.AnyAsync(u => u.Id != id && u.Email == request.Email);
            if (existEmail) throw new ConflictException("El correo no esta disponible.");

            // crear password hash
            var passwordHash = request.Password!;

            user.Update(request.FullName!, request.Email!, passwordHash, request.Role!.Value);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("El usuario no esta en el sistema.");

            var hasTicktets = await _context.Tickets.AnyAsync(t => t.CreatedByUserId == id || t.AssignedToUserId == id);
            if (hasTicktets) throw new ConflictException("No se puede eliminar un Usuario con tickets creados o asignados.");

            _context.Remove(user);
            await _context.SaveChangesAsync();
        }
        public async Task Activate(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("El usuario no esta en el sistema.");

            user.Activate();
            await _context.SaveChangesAsync();
        }
        public async Task Deactivate(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("El usuario no esta en el sistema.");

            user.Deactivate();
            await _context.SaveChangesAsync();
        }
        public async Task<UserDto> GetById(Guid id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new NotFoundException("El usuario no esta en el sistema.");
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
            };
        }
        public async Task<IEnumerable<UserDto>> GetAll(UserQuery paramsQuery)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            if (!String.IsNullOrWhiteSpace(paramsQuery.name))
            {
                query = query.Where(u => u.FullName.ToLower().Contains(paramsQuery.name.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(paramsQuery.email))
            {
                query = query.Where(u => u.Email.ToLower().Contains(paramsQuery.email.ToLower()));
            }
            if (paramsQuery.role.HasValue)
            {
                query = query.Where(u => u.Role == paramsQuery.role.Value);
            }
            if (paramsQuery.isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == paramsQuery.isActive.Value);
            }

            return await query.Select(u => new UserDto
            {
                Id = u.Id,
                FullName= u.FullName,
                Email= u.Email,
                Role= u.Role,
                IsActive= u.IsActive,
                CreatedAt = u.CreatedAt,
            }).ToListAsync();
        }
    }
}
