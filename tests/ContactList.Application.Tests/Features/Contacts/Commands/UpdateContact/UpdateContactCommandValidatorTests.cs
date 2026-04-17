using ContactList.Application.Features.Contacts.Commands.UpdateContact;
using FluentValidation.TestHelper;

namespace ContactList.Application.Tests.Features.Contacts.Commands.UpdateContact
{
    public class UpdateContactCommandValidatorTests
    {
        private readonly UpdateContactCommandValidator _validator = new();

        private static UpdateContactCommand ValidCommand() => new(
            Id: Guid.NewGuid(),
            FirstName: "Jan",
            LastName: "Kowalski",
            Email: "jan.kowalski@example.com",
            Password: null,
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateOnly(1990, 1, 1),
            CategoryId: Guid.NewGuid(),
            SubcategoryId: null,
            SubcategoryName: null);

        [Fact]
        public void Validate_ValidCommandWithNullPassword_ReturnsNoErrors()
        {
            // Arrange
            var cmd = ValidCommand();

            // Act & Assert
            _validator.TestValidate(cmd).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ValidCommandWithNewPassword_ReturnsNoErrors()
        {
            // Arrange
            var cmd = ValidCommand() with { Password = "Password1!" };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_EmptyId_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { Id = Guid.Empty };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Validate_NullPassword_ReturnsNoErrors()
        {
            // Arrange
            var cmd = ValidCommand() with { Password = null };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.Password!);
        }

        [Theory]
        [InlineData("Short1!")]
        [InlineData("alllowercase1!")]
        [InlineData("ALLUPPERCASE1!")]
        [InlineData("NoDigitsHere!")]
        [InlineData("NoSpecial123")]
        public void Validate_SuppliedPasswordViolatingComplexity_ReturnsValidationError(string value)
        {
            // Arrange
            var cmd = ValidCommand() with { Password = value };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Password!);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyFirstName_ReturnsValidationError(string value)
        {
            // Arrange
            var cmd = ValidCommand() with { FirstName = value };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.FirstName);
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

        [Theory]
        [InlineData("")]
        [InlineData("123456789")]
        [InlineData("+abc")]
        public void Validate_InvalidPhoneNumber_ReturnsValidationError(string value)
        {
            // Arrange
            var cmd = ValidCommand() with { PhoneNumber = value };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Fact]
        public void Validate_FutureDateOfBirth_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        [Fact]
        public void Validate_DateOfBirthBefore1900_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { DateOfBirth = new DateOnly(1899, 12, 31) };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        [Fact]
        public void Validate_EmptyCategoryId_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { CategoryId = Guid.Empty };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        [Fact]
        public void Validate_SubcategoryNameOver100Chars_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { SubcategoryName = new string('a', 101) };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.SubcategoryName);
        }
    }
}