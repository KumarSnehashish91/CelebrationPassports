using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using CelebrationPassports.Application.Features.Authentication.Services;
using CelebrationPassports.Application.Features.Authentication.DTOs;

using Moq;
using Xunit;
using CelebrationPassports.Application.Exceptions;

namespace CelebrationPassports.UnitTests.Authentication;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task Register_Should_Throw_DuplicateEmailException_When_Email_Already_Exists()
    {
        var userRepository = new Mock<IUserRepository>();

        var unitOfWork = new Mock<IUnitOfWork>();

        var passwordHasher = new Mock<IPasswordHasher>();

        userRepository
    .Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
    .ReturnsAsync(true);
        var service = new AuthenticationService(
             userRepository.Object,
             unitOfWork.Object,
             passwordHasher.Object);

        var request = new RegisterRequest
        {
            Email = "kumar@test.com",
            Password = "Welcome@123",
            ConfirmPassword = "Welcome@123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEmailException>(
            () => service.RegisterAsync(request));

        userRepository.Verify(
    x => x.EmailExistsAsync(request.Email),
    Times.Once);
    }
}