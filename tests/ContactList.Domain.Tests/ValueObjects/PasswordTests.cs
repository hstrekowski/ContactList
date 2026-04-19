using FluentAssertions;
using ContactList.Domain.ValueObjects;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Tests.ValueObjects
{
    public class PasswordTests
    {
        [Theory]
        [InlineData("Valid123!")]
        [InlineData("P@ssw0rd2026")]
        [InlineData("A1!bcdef")]
        public void Constructor_WithValidPassword_ShouldCreateInstanceAndSetValue(string validPassword)
        {
            // Arrange & Act
            var password = new Password(validPassword);

            // Assert
            password.Value.Should().Be(validPassword);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyValue_ShouldThrowDomainException(string? invalidPassword)
        {
            // Arrange & Act
            Action act = () => new Password(invalidPassword!);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Password cannot be empty.");
        }

        [Fact]
        public void Constructor_WithLengthExactlyMinLength_ShouldNotThrow()
        {
            // Arrange
            var exactLengthPassword = "A1b!cdef";

            // Act
            var password = new Password(exactLengthPassword);

            // Assert
            password.Value.Should().Be(exactLengthPassword);
        }

        [Fact]
        public void Constructor_WithLengthOneCharBelowMinLength_ShouldThrowDomainException()
        {
            // Arrange
            var tooShortPassword = "A1b!cde";

            // Act
            Action act = () => new Password(tooShortPassword);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Password must be at least 8 characters long.");
        }

        [Fact]
        public void Constructor_WithLengthExactlyMaxLength_ShouldNotThrow()
        {
            // Arrange
            var basePart = "A1b!";
            var padding = new string('c', 124);
            var exactLengthPassword = $"{basePart}{padding}";

            // Act
            var password = new Password(exactLengthPassword);

            // Assert
            password.Value.Should().Be(exactLengthPassword);
        }

        [Fact]
        public void Constructor_WithLengthOneCharOverMaxLength_ShouldThrowDomainException()
        {
            // Arrange
            var basePart = "A1b!";
            var padding = new string('c', 125);
            var tooLongPassword = $"{basePart}{padding}";

            // Act
            Action act = () => new Password(tooLongPassword);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Password cannot be longer than 128 characters.");
        }

        [Theory]
        [InlineData("valid123!", "Password must contain at least one uppercase letter.")]
        [InlineData("VALID123!", "Password must contain at least one lowercase letter.")]
        [InlineData("ValidPassword!", "Password must contain at least one digit.")]
        [InlineData("Valid12345678", "Password must contain at least one special character.")]
        public void Constructor_WithoutRequiredComplexity_ShouldThrowDomainException(string invalidPassword, string expectedMessage)
        {
            // Arrange & Act
            Action act = () => new Password(invalidPassword);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void ToString_ShouldReturnMaskedValue()
        {
            // Arrange
            var password = new Password("Valid123!");

            // Act
            var result = password.ToString();

            // Assert
            result.Should().Be("********");
        }

        [Fact]
        public void RecordEquality_WithSameValue_ShouldReturnTrue()
        {
            // Arrange
            var pass1 = new Password("Valid123!");
            var pass2 = new Password("Valid123!");

            // Act
            var areEqual = pass1 == pass2;

            // Assert
            areEqual.Should().BeTrue();
            pass1.Should().Be(pass2);
        }

        [Fact]
        public void RecordInequality_WithDifferentValues_ShouldReturnTrue()
        {
            // Arrange
            var pass1 = new Password("Valid123!");
            var pass2 = new Password("An0therV@lid!");

            // Act
            var areDifferent = pass1 != pass2;

            // Assert
            areDifferent.Should().BeTrue();
            pass1.Should().NotBe(pass2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForSamePasswordValue()
        {
            // Arrange
            var pass1 = new Password("Valid123!");
            var pass2 = new Password("Valid123!");

            // Act
            var hashCode1 = pass1.GetHashCode();
            var hashCode2 = pass2.GetHashCode();

            // Assert
            hashCode1.Should().Be(hashCode2);
        }
    }
}