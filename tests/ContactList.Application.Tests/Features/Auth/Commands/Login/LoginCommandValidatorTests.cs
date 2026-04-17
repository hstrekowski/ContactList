using ContactList.Application.Features.Auth.Commands.Login;
using FluentValidation.TestHelper;

namespace ContactList.Application.Tests.Features.Auth.Commands.Login
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator = new();

        private static LoginCommand ValidCommand() =>
            new(Email: "user@example.com", Password: "anything");

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            // Arrange
            var cmd = ValidCommand();

            // Act & Assert
            _validator.TestValidate(cmd).ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        public void Validate_InvalidEmail_ReturnsValidationError(string value)
        {
            // Arrange
            var cmd = ValidCommand() with { Email = value };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_EmptyPassword_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { Password = "" };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Validate_WeakPassword_ReturnsNoErrors()
        {
            // Arrange
            var cmd = ValidCommand() with { Password = "x" };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}