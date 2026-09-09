using SecureIssueTrackerApi_07.Application;
using SecureIssueTrackerApi_07.Application.Security;
using SecureIssueTrackerApi_07.Domain;
using SecureIssueTrackerApi_07.Dtos.Auth;
using SecureIssueTrackerApi_07.Exceptions;

namespace SecureIssueTrackerApi_07.Tests.Application
{
    public class AuthUseCaseTests
    {
        [Fact]
        public async Task Register_ShouldCreateCustomer_WhenEmailIsUnique()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var existingUser = new User("Tests User", "test@example.com", "password", UserRole.Customer);

            context.Users.Add(existingUser);
            await context.SaveChangesAsync();
            var passwordHashService = new FakePasswordHashService();
            var jwtTokenGenerator = new FakeJwtTokenGenerator();
            var useCase = new AuthUseCase(context, passwordHashService, jwtTokenGenerator);
            var request = new RegisterCustomerRequest
            {
                FullName = "José Gallardo",
                Email = "jose@test.com",
                Password = "123456"
            };

            // Act
            var result = await useCase.RegisterCustomer(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
            Assert.Equal("José Gallardo", result.FullName);
            Assert.Equal("jose@test.com", result.Email);
            Assert.Equal(UserRole.Customer, result.Role);

            var userInDb = await context.Users.FindAsync(result.UserId);

            Assert.NotNull(userInDb);
            Assert.Equal("José Gallardo", userInDb.FullName);
            Assert.Equal("jose@test.com", userInDb.Email);
            Assert.Equal(UserRole.Customer, userInDb.Role);
            Assert.True(userInDb.IsActive);

            Assert.Equal("HASHED:123456", userInDb.PasswordHash);
            Assert.NotEqual("123456", userInDb.PasswordHash);
        }
        [Fact]
        public async Task Register_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            // Arrange
            using var db = new TestDbContextFactory();
            var context = db.Context;

            var existingUser = new User(
                "José Gallardo",
                "jose@test.com",
                "HASHED:123456",
                UserRole.Customer);

            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var passwordHashService = new FakePasswordHashService();
            var jwtTokenGenerator = new FakeJwtTokenGenerator();

            var useCase = new AuthUseCase(
                context,
                passwordHashService,
                jwtTokenGenerator);

            var request = new RegisterCustomerRequest
            {
                FullName = "Otro Usuario",
                Email = "jose@test.com",
                Password = "abcdef"
            };

            // Act
            Func<Task> act = () => useCase.RegisterCustomer(request);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }
    }
    public sealed class FakePasswordHashService : IPasswordHashService
    {
        public string Hash(string password)
        {
            return $"HASHED:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == $"HASHED:{password}";
        }
    }
    public sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        public string Generate(User user)
        {
            return $"fake-token-for-{user.Id}";
        }
    }
}
