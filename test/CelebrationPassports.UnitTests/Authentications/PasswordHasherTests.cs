using CelebrationPassports.Infrastructure.Authentication.Services;
using FluentAssertions;

namespace CelebrationPassports.UnitTests.Authentications;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_Should_Create_A_Valid_Hash()
    {
        // Arrange
        var passwordHasher = new PasswordHasher();
        const string password = "Welcome@123";

        // Act
        var hash = passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrWhiteSpace();
        hash.Should().NotBe(password);
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_For_Correct_Password()
    {
        // Arrange
        var passwordHasher = new PasswordHasher();
        const string password = "Welcome@123";
        var hash = passwordHasher.HashPassword(password);

        // Act
        var result = passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False_For_Wrong_Password()
    {
        // Arrange
        var passwordHasher = new PasswordHasher();
        var hash = passwordHasher.HashPassword("Welcome@123");

        // Act
        var result = passwordHasher.VerifyPassword("WrongPassword", hash);

        // Assert
        result.Should().BeFalse();
    }
}