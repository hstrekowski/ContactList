using ContactList.Application.Features.Contacts.Commands.CreateContact;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ContactList.Application.Tests.Features.Contacts.Commands.CreateContact
{
    public class CreateContactCommandValidatorTests
    {
        private readonly CreateContactCommandValidator _validator = new();

        private static CreateContactCommand ValidCommand() => new(
            FirstName: "Jan",
            LastName: "Kowalski",
            Email: "jan.kowalski@example.com",
            Password: "Password1!",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateOnly(1990, 1, 1),
            CategoryId: Guid.NewGuid(),
            SubcategoryId: null,
            SubcategoryName: null);

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
        [InlineData("   ")]
        public void Validate_EmptyFirstName_ReturnsValidationError(string value)
        {
            // Arrange
            var cmd = ValidCommand() with { FirstName = value };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Validate_FirstNameOver100Chars_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { FirstName = new string('a', 101) };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyLastName_ReturnsValidationError(string value)
        {
            // Arrange
            var cmd = ValidCommand() with { LastName = value };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public void Validate_LastNameOver100Chars_ReturnsValidationError()
        {
            // Arrange
            var cmd = ValidCommand() with { LastName = new string('a', 101) };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.LastName);
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

        [Fact]
        public void Validate_NullSubcategoryName_ReturnsNoErrors()
        {
            // Arrange
            var cmd = ValidCommand() with { SubcategoryName = null };

            // Act & Assert
            _validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.SubcategoryName);
        }
    }
}