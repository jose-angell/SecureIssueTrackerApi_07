using Microsoft.EntityFrameworkCore;
using SecureIssueTrackerApi_07.Domain;
using SecureIssueTrackerApi_07.Dtos.Ticket;
using SecureIssueTrackerApi_07.Exceptions;
using SecureIssueTrackerApi_07.Infrastructure;
using System.Net.Sockets;

namespace SecureIssueTrackerApi_07.Application
{
    public class TicketUserCase
    {
        private readonly AppDbContext _context;
        public TicketUserCase(AppDbContext context)
        {
            _context = context;
        }
        public async Task<TicketDto> Create(CreateTicketRequest request)
        {
            //TODO: agregar la funcion para tommar el id de la persona desde HTTContext
            var userId = Guid.NewGuid();
            var newTicket = new Ticket(request.Title!, request.Description!, request.Priority!.Value, userId);
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
        public async Task InProgress(Guid id)
        {
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

            var existUser = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!existUser) throw new NotFoundException("El usuario no esta en el sistema.");

            ticket.AssignTo(userId);
            await _context.SaveChangesAsync();
        }
        public async Task<TicketDto> GetById(Guid id)
        {
            var ticket = await _context.Tickets.AsNoTracking()
                .Include(u => u.CreatedByUser)
                .Include(u => u.AssignedToUser)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (ticket == null) throw new NotFoundException("El ticket no esta en el sistema.");
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
            IQueryable<Ticket> query = _context.Tickets.AsNoTracking()
                .Include(u => u.CreatedByUser)
                .Include(u => u.AssignedToUser);

            if (String.IsNullOrWhiteSpace(paramsQuery.Title))
            {
                query = query.Where(t => t.Title.ToLower().Contains(paramsQuery.Title!));
            }
            if (String.IsNullOrWhiteSpace(paramsQuery.Description))
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
