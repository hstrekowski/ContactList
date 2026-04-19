using FluentAssertions;
using ContactList.Domain.ValueObjects;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Theory]
        [InlineData("test@example.com", "test@example.com")]
        [InlineData("  TEST@EXAMPLE.COM  ", "test@example.com")]
        [InlineData("first.last@domain.co.uk", "first.last@domain.co.uk")]
        [InlineData("123@123.com", "123@123.com")]
        public void Constructor_WithValidEmail_ShouldCreateAndNormalizeValue(string input, string expected)
        {
            // Arrange & Act
            var email = new Email(input);

            // Assert
            email.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyValue_ShouldThrowDomainException(string? invalidEmail)
        {
            // Arrange & Act
            Action act = () => new Email(invalidEmail!);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Email cannot be empty.");
        }

        [Fact]
        public void Constructor_WithLengthExactlyMaxLength_ShouldNotThrow()
        {
            // Arrange
            var localPart = new string('a', 245);
            var exactLengthEmail = $"{localPart}@test.com";

            // Act
            var email = new Email(exactLengthEmail);

            // Assert
            email.Value.Should().Be(exactLengthEmail);
        }

        [Fact]
        public void Constructor_WithLengthOneCharOverMaxLength_ShouldThrowDomainException()
        {
            // Arrange
            var localPart = new string('a', 246);
            var tooLongEmail = $"{localPart}@test.com";

            // Act
            Action act = () => new Email(tooLongEmail);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Email cannot be longer than 254 characters.");
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("@missingusername.com")]
        [InlineData("username@.com")]
        [InlineData("username@domain")]
        [InlineData("username@domain.c")]
        [InlineData("user name@domain.com")]
        [InlineData("username@do_main.com")]
        [InlineData("user..name@domain.com")]
        public void Constructor_WithInvalidFormat_ShouldThrowDomainException(string invalidEmail)
        {
            // Arrange & Act
            Action act = () => new Email(invalidEmail);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Email format is invalid.");
        }

        [Fact]
        public void ToString_ShouldReturnEmailValue()
        {
            // Arrange
            var emailString = "test@example.com";
            var email = new Email(emailString);

            // Act
            var result = email.ToString();

            // Assert
            result.Should().Be(emailString);
        }

        [Fact]
        public void RecordEquality_WithSameValue_ShouldReturnTrue()
        {
            // Arrange
            var email1 = new Email("Test@Example.com");
            var email2 = new Email("test@example.com  ");

            // Act
            var areEqual = email1 == email2;

            // Assert
            areEqual.Should().BeTrue();
            email1.Should().Be(email2);
        }

        [Fact]
        public void RecordInequality_WithDifferentValues_ShouldReturnTrue()
        {
            // Arrange
            var email1 = new Email("test1@example.com");
            var email2 = new Email("test2@example.com");

            // Act
            var areDifferent = email1 != email2;

            // Assert
            areDifferent.Should().BeTrue();
            email1.Should().NotBe(email2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForSameEmailValue()
        {
            // Arrange
            var email1 = new Email("Test@Example.com");
            var email2 = new Email("test@example.com");

            // Act
            var hashCode1 = email1.GetHashCode();
            var hashCode2 = email2.GetHashCode();

            // Assert
            hashCode1.Should().Be(hashCode2);
        }
    }
}