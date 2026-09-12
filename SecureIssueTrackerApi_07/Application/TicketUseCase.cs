using Microsoft.EntityFrameworkCore;
using SecureIssueTrackerApi_07.Application.Security;
using SecureIssueTrackerApi_07.Domain;
using SecureIssueTrackerApi_07.Dtos.Ticket;
using SecureIssueTrackerApi_07.Exceptions;
using SecureIssueTrackerApi_07.Infrastructure;
using System.Net.Sockets;

namespace SecureIssueTrackerApi_07.Application
{
    public class TicketUseCase
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public TicketUseCase(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<TicketDto> Create(CreateTicketRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(currentUserId);

            if (user is null) throw new NotFoundException("Usuario no encontrado.");

            if (!user.IsActive) throw new ForbiddenException("El usuario está inactivo.");

            var newTicket = new Ticket(request.Title!, request.Description!, request.Priority!.Value, currentUserId);
            await _context.Tickets.AddAsync(newTicket);
            await _context.SaveChangesAsync();
            return new TicketDto
            {
                Id = newTicket.Id,
                Title = newTicket.Title,
                Description = newTicket.Description,
                Status = newTicket.Status,
                Priority = newTicket.Priority,
                CreatedAt = newTicket.CreatedAt,
                CreatedByUserId = newTicket.CreatedByUserId,
            };
        }
        public async Task UpdateDescription(Guid id, string description)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            ticket.UpdateDescription(description);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            _context.Remove(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task Open(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            ticket.Open();
            await _context.SaveChangesAsync();
        }
        public async Task StartProgress(Guid id)
        {
            var currentRole = _currentUserService.Role;
            if(currentRole == UserRole.Customer)
                throw new ForbiddenException("Un cliente no puede iniciar el progreso de un ticket.");

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            ticket.InProgress();
            await _context.SaveChangesAsync();
        }
        public async Task Resolved(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            ticket.Resolved();
            await _context.SaveChangesAsync();
        }
        public async Task Closed(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            ticket.Closed();
            await _context.SaveChangesAsync();
        }
        public async Task AssignTo(Guid id, Guid userId)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            var targetUser = await _context.Users.FindAsync(userId);
            if (targetUser is null)
                throw new NotFoundException("El usuario no está en el sistema.");

            if (!targetUser.IsActive)
                throw new ConflictException("No se puede asignar un ticket a un usuario inactivo.");

            if (targetUser.Role != UserRole.Agent)
                throw new ConflictException("El ticket solo puede asignarse a un agente.");

            var currentUserId = _currentUserService.UserId;
            var currentRole = _currentUserService.Role;

            if (currentRole == UserRole.Agent && userId != currentUserId)
                throw new ForbiddenException("Un agente solo puede asignarse tickets a sí mismo.");

            if (currentRole != UserRole.Agent && currentRole != UserRole.Admin)
                throw new ForbiddenException("No tienes permiso para asignar tickets.");

            ticket.AssignTo(userId);
            await _context.SaveChangesAsync();
        }
        private void EnsureCanViewTicket(Ticket ticket)
        {
            var currentUserId = _currentUserService.UserId;
            var currentRole = _currentUserService.Role;

            if (currentRole == UserRole.Admin)
                return;

            if (currentRole == UserRole.Customer &&
                ticket.CreatedByUserId == currentUserId)
                return;

            if (currentRole == UserRole.Agent &&
                (ticket.Status == TicketStatus.Open ||
                 ticket.AssignedToUserId == currentUserId))
                return;

            throw new ForbiddenException("No tienes permiso para ver este ticket.");
        }
        public async Task<TicketDto> GetById(Guid id)
        {
            var ticket = await _context.Tickets.AsNoTracking()
                .Include(u => u.CreatedByUser)
                .Include(u => u.AssignedToUser)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");

            EnsureCanViewTicket(ticket);

            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority,
                CreatedByUserId = ticket.CreatedByUserId,
                CreatedByUserName = ticket.CreatedByUser.FullName,
                AssignedToUserId = ticket.AssignedToUserId,
                AssignedToUserName = ticket.AssignedToUser != null ? ticket.AssignedToUser.FullName : "",
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt
            };
        }
        public async Task<IEnumerable<TicketDto>> GetAll(TicketQuery paramsQuery)
        {
            var currentUserId = _currentUserService.UserId;
            var currentRole = _currentUserService.Role;

            IQueryable<Ticket> query = _context.Tickets.AsNoTracking()
                .Include(u => u.CreatedByUser)
                .Include(u => u.AssignedToUser);

            if (currentRole == UserRole.Customer)
            {
                query = query.Where(t => t.CreatedByUserId == currentUserId);
            }
            else if (currentRole == UserRole.Agent)
            {
                query = query.Where(t =>
                    //t.Status == TicketStatus.Open ||
                    t.AssignedToUserId == currentUserId);
            }

            if (!String.IsNullOrWhiteSpace(paramsQuery.Title))
            {
                query = query.Where(t => t.Title.ToLower().Contains(paramsQuery.Title!));
            }
            if (!String.IsNullOrWhiteSpace(paramsQuery.Description))
            {
                query = query.Where(t => t.Description.ToLower().Contains(paramsQuery.Description!));
            }
            if (paramsQuery.Status.HasValue)
            {
                query = query.Where(u => u.Status == paramsQuery.Status.Value);
            }
            if (paramsQuery.Priority.HasValue)
            {
                query = query.Where(u => u.Priority == paramsQuery.Priority.Value);
            }
            if (paramsQuery.CreatedByUserId.HasValue)
            {
                query = query.Where(u => u.CreatedByUserId == paramsQuery.CreatedByUserId.Value);
            }
            if (paramsQuery.AssignedToUserId.HasValue)
            {
                query = query.Where(u => u.AssignedToUserId == paramsQuery.AssignedToUserId.Value);
            }
            if (paramsQuery.FromDate.HasValue)
            {
                query = query.Where(u => u.CreatedAt >= paramsQuery.FromDate.Value);
            }
            if (paramsQuery.ToDate.HasValue)
            {
                query = query.Where(u => u.CreatedAt <= paramsQuery.ToDate.Value);
            }

            return await query.Select(ticket => new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority,
                CreatedByUserId = ticket.CreatedByUserId,
                CreatedByUserName = ticket.CreatedByUser.FullName,
                AssignedToUserId = ticket.AssignedToUserId,
                AssignedToUserName = ticket.AssignedToUser != null ? ticket.AssignedToUser.FullName : "",
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt
            }).ToArrayAsync();
        }
    }
}
