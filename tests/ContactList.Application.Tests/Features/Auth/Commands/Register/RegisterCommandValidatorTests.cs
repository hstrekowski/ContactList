using ContactList.Application.Features.Auth.Commands.Register;
using FluentValidation.TestHelper;

namespace ContactList.Application.Tests.Features.Auth.Commands.Register
{
    public class RegisterCommandValidatorTests
    {
        private readonly RegisterCommandValidator _validator = new();

        private static RegisterCommand ValidCommand() =>
            new(Email: "user@example.com", Password: "Password1!");

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
        public void Validate_EmailOver254Chars_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { Email = new string('a', 250) + "@x.pl" };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Short1!")]
        [InlineData("alllowercase1!")]
        [InlineData("ALLUPPERCASE1!")]
        [InlineData("NoDigitsHere!")]
        [InlineData("NoSpecial123")]
        public void Validate_PasswordViolatingComplexity_ReturnsValidationError(string value)
        {
            // Arrange
            var cmd = ValidCommand() with { Password = value };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Validate_PasswordOver128Chars_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { Password = new string('A', 120) + "aa1!" + new string('b', 10) };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}