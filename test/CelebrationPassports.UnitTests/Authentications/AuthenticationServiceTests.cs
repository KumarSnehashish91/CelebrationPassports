using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using CelebrationPassports.Application.Authentication.Services;
using CelebrationPassports.Application.Authentication.DTOs.RequestDTO;

using FluentValidation;
using FluentValidation.Results;
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
        var userLoginHistory = new Mock<IUserLoginHistoryRepository>();
        var usersession = new Mock<IUserSessionRepository>();
        var userProfileRepository = new Mock<IUserProfileRepository>();
        var tokenService = new Mock<ITokenService>();
        var registerValidator = new Mock<IValidator<RegisterRequest>>();
        var loginValidator = new Mock<IValidator<LoginRequest>>();

        registerValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RegisterRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        userRepository
    .Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
    .ReturnsAsync(true);
        var service = new AuthenticationService(
             userRepository.Object,
             unitOfWork.Object,
             passwordHasher.Object,
             userLoginHistory.Object,
             usersession.Object,
             userProfileRepository.Object,
             tokenService.Object,
             registerValidator.Object,
             loginValidator.Object);

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