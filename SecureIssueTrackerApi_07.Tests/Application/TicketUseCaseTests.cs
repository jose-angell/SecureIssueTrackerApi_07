using SecureIssueTrackerApi_07.Application;
using SecureIssueTrackerApi_07.Application.Security;
using SecureIssueTrackerApi_07.Domain;
using SecureIssueTrackerApi_07.Dtos.Ticket;
using SecureIssueTrackerApi_07.Exceptions;

namespace SecureIssueTrackerApi_07.Tests.Application
{
    public class TicketUseCaseTests
    {
        [Fact]
        public async Task Create_ShouldCreateTicket_ForCurrentAuthenticatedUser()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var currentUserService = new FakeCurrentUserService();
            var currentUser = currentUserService.GetCurrentUser();

            context.Users.Add(currentUser);
            await context.SaveChangesAsync();

            var useCase = new TicketUseCase(context, currentUserService);

            var request = new CreateTicketRequest
            {
                Title = "Test Ticket",
                Description = "This is a test ticket.",
                Priority = TicketPriority.Medium
            };

            // Act
            var result = await useCase.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Ticket", result.Title);
            Assert.Equal("This is a test ticket.", result.Description);
            Assert.Equal(TicketPriority.Medium, result.Priority);
            Assert.Equal(TicketStatus.Open, result.Status);
            Assert.Equal(currentUser.Id, result.CreatedByUserId);

            var ticketInDb = await context.Tickets.FindAsync(result.Id);

            Assert.NotNull(ticketInDb);
            Assert.Equal(currentUser.Id, ticketInDb.CreatedByUserId);
            Assert.Equal(TicketStatus.Open, ticketInDb.Status);
        }
        [Fact]
        public async Task GetAll_ShouldReturnOnlyOwnTickets_WhenCurrentUserIsCustomer()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var currentUserService = new FakeCurrentUserService();
            var otherUser = new User("user2", "user2@example.com", "password", UserRole.Agent);
            var ticketOne = new Ticket("Ticket 1", "Description 1", TicketPriority.Low, otherUser.Id);
            var ticketTwo = new Ticket("Ticket 2", "Description 2", TicketPriority.Low, otherUser.Id);
            var ticketThree = new Ticket("Ticket 3", "Description 3", TicketPriority.Low, currentUserService.UserId);
            var ticketFour = new Ticket("Ticket 4", "Description 4", TicketPriority.Low, currentUserService.UserId);
            var ticketFive = new Ticket("Ticket 5", "Description 5", TicketPriority.Low, currentUserService.UserId);


            var currentUser = currentUserService.GetCurrentUser();

            context.Users.Add(currentUser);
            context.Users.Add(otherUser);

            context.Tickets.Add(ticketOne);
            context.Tickets.Add(ticketTwo);
            context.Tickets.Add(ticketThree);
            context.Tickets.Add(ticketFour);
            context.Tickets.Add(ticketFive);
            await context.SaveChangesAsync();

            var useCase = new TicketUseCase(context, currentUserService);

            var query = new TicketQuery();

            // Act
            var result = await useCase.GetAll(query);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Equal(ticketThree.CreatedByUserId, resultList[0].CreatedByUserId);
            Assert.Equal(ticketFour.CreatedByUserId, resultList[1].CreatedByUserId);
            Assert.Equal(ticketFive.CreatedByUserId, resultList[2].CreatedByUserId);

        }
        [Fact]
        public async Task GetAll_ShouldReturnAllTickets_WhenCurrentUserIsAdmin()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var currentUserService = new FakeCurrentUserService();
            var agentUserService = new FakeCurrentUserServiceAgent();
            var adminUserService = new FakeCurrentUserServiceAdmin();
            var otherUser = new User("user2", "user2@example.com", "password", UserRole.Customer);
            var ticketOne = new Ticket("Ticket 1", "Description 1", TicketPriority.Low, otherUser.Id);
            var ticketTwo = new Ticket("Ticket 2", "Description 2", TicketPriority.Low, otherUser.Id);
            var ticketThree = new Ticket("Ticket 3", "Description 3", TicketPriority.Low, currentUserService.UserId);
            var ticketFour = new Ticket("Ticket 4", "Description 4", TicketPriority.Low, currentUserService.UserId);
            var ticketFive = new Ticket("Ticket 5", "Description 5", TicketPriority.Low, currentUserService.UserId);

            ticketThree.AssignTo(agentUserService.UserId);
            ticketFour.AssignTo(agentUserService.UserId);

            var currentUser = currentUserService.GetCurrentUser();
            var agentUser = agentUserService.GetCurrentUser();
            var adminUser = adminUserService.GetCurrentUser();

            context.Users.Add(currentUser);
            context.Users.Add(adminUser);
            context.Users.Add(agentUser);
            context.Users.Add(otherUser);

            context.Tickets.Add(ticketOne);
            context.Tickets.Add(ticketTwo);
            context.Tickets.Add(ticketThree);
            context.Tickets.Add(ticketFour);
            context.Tickets.Add(ticketFive);

            await context.SaveChangesAsync();

            var useCase = new TicketUseCase(context, adminUserService);

            var query = new TicketQuery();

            // Act
            var result = await useCase.GetAll(query);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(5, resultList.Count);
        }
        [Fact]
        public async Task GetAll_ShouldReturnOpenAndAssignedTickets_WhenCurrentUserIsAgent()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var currentUserService = new FakeCurrentUserService();
            var agentUserService = new FakeCurrentUserServiceAgent();
            var otherUser = new User("user2", "user2@example.com", "password", UserRole.Customer);
            var ticketOne = new Ticket("Ticket 1", "Description 1", TicketPriority.Low, otherUser.Id);
            var ticketTwo = new Ticket("Ticket 2", "Description 2", TicketPriority.Low, otherUser.Id);
            var ticketThree = new Ticket("Ticket 3", "Description 3", TicketPriority.Low, currentUserService.UserId);
            var ticketFour = new Ticket("Ticket 4", "Description 4", TicketPriority.Low, currentUserService.UserId);
            var ticketFive = new Ticket("Ticket 5", "Description 5", TicketPriority.Low, currentUserService.UserId);

            ticketThree.AssignTo(agentUserService.UserId);
            ticketFour.AssignTo(agentUserService.UserId);

            var currentUser = currentUserService.GetCurrentUser();
            var agentUser = agentUserService.GetCurrentUser();

            context.Users.Add(currentUser);
            context.Users.Add(agentUser);
            context.Users.Add(otherUser);

            context.Tickets.Add(ticketOne);
            context.Tickets.Add(ticketTwo);
            context.Tickets.Add(ticketThree);
            context.Tickets.Add(ticketFour);
            context.Tickets.Add(ticketFive);

            await context.SaveChangesAsync();

            var useCase = new TicketUseCase(context, agentUserService);

            var query = new TicketQuery();

            // Act
            var result = await useCase.GetAll(query);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(ticketThree.AssignedToUserId, resultList[0].AssignedToUserId);
            Assert.Equal(ticketFour.AssignedToUserId, resultList[1].AssignedToUserId);
        }
        [Fact]
        public async Task GetById_ShouldThrowForbiddenException_WhenCustomerTriesToAccessOtherUserTicket()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var currentUserService = new FakeCurrentUserService();
            var otherUser = new User("user2", "user2@example.com", "password", UserRole.Customer);
            var ticketOne = new Ticket("Ticket 1", "Description 1", TicketPriority.Low, otherUser.Id);
            var ticketTwo = new Ticket("Ticket 2", "Description 2", TicketPriority.Low, otherUser.Id);
            var ticketThree = new Ticket("Ticket 3", "Description 3", TicketPriority.Low, currentUserService.UserId);
            var ticketFour = new Ticket("Ticket 4", "Description 4", TicketPriority.Low, currentUserService.UserId);
            var ticketFive = new Ticket("Ticket 5", "Description 5", TicketPriority.Low, currentUserService.UserId);


            var currentUser = currentUserService.GetCurrentUser();

            context.Users.Add(currentUser);
            context.Users.Add(otherUser);

            context.Tickets.Add(ticketOne);
            context.Tickets.Add(ticketTwo);
            context.Tickets.Add(ticketThree);
            context.Tickets.Add(ticketFour);
            context.Tickets.Add(ticketFive);

            await context.SaveChangesAsync();

            var useCase = new TicketUseCase(context, currentUserService);


            // Act
            Func<Task> act = () => useCase.GetById(ticketOne.Id);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(act);
        }
        [Fact]
        public async Task AssignTo_ShouldThrowConflictException_WhenTargetUserIsNotAgent()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var currentUserService = new FakeCurrentUserService();
            var ticket= new Ticket("Ticket ", "Description ", TicketPriority.Low, currentUserService.UserId);


            var currentUser = currentUserService.GetCurrentUser();

            context.Users.Add(currentUser);

            context.Tickets.Add(ticket);

            await context.SaveChangesAsync();

            var useCase = new TicketUseCase(context, currentUserService);


            // Act
            Func<Task> act = () => useCase.AssignTo(ticket.Id, currentUser.Id);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }
        
        private sealed class FakeCurrentUserService : ICurrentUserService
        {
            private readonly User _currentUser = new(
                "John Doe",
                "john@test.com",
                "hashed-password",
                UserRole.Customer);

            public User GetCurrentUser()
            {
                return _currentUser;
            }

            public bool IsAuthenticated => true;

            public Guid UserId => _currentUser.Id;

            public string? Email => _currentUser.Email;

            public UserRole Role => _currentUser.Role;
        }
        private sealed class FakeCurrentUserServiceAgent : ICurrentUserService
        {
            private readonly User _currentUser = new(
                "Agent test",
                "agent@test.com",
                "hashed-password",
                UserRole.Agent);

            public User GetCurrentUser()
            {
                return _currentUser;
            }

            public bool IsAuthenticated => true;

            public Guid UserId => _currentUser.Id;

            public string? Email => _currentUser.Email;

            public UserRole Role => _currentUser.Role;
        }
        private sealed class FakeCurrentUserServiceAdmin : ICurrentUserService
        {
            private readonly User _currentUser = new(
                "Admin test",
                "admin@test.com",
                "hashed-password",
                UserRole.Admin);

            public User GetCurrentUser()
            {
                return _currentUser;
            }

            public bool IsAuthenticated => true;

            public Guid UserId => _currentUser.Id;

            public string? Email => _currentUser.Email;

            public UserRole Role => _currentUser.Role;
        }
    }
}
