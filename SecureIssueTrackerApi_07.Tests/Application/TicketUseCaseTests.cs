using SecureIssueTrackerApi_07.Application;
using SecureIssueTrackerApi_07.Application.Security;
using SecureIssueTrackerApi_07.Domain;
using SecureIssueTrackerApi_07.Dtos.Ticket;

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
    }
}
