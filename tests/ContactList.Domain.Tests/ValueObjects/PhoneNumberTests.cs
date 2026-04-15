using FluentAssertions;
using ContactList.Domain.ValueObjects;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Tests.ValueObjects
{
    public class PhoneNumberTests
    {
        [Theory]
        [InlineData("+48123456789", "+48123456789")]
        [InlineData("+48 123 456 789", "+48123456789")]
        [InlineData("+1-800-555-1234", "+18005551234")]
        [InlineData("+44 7911-123456", "+447911123456")]
        public void Constructor_WithValidValue_ShouldCreateAndNormalize(string input, string expected)
        {
            // Arrange & Act
            var phoneNumber = new PhoneNumber(input);

            // Assert
            phoneNumber.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyValue_ShouldThrowDomainException(string invalidNumber)
        {
            // Arrange & Act
            Action act = () => new PhoneNumber(invalidNumber);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Phone number cannot be empty.");
        }

        [Theory]
        [InlineData("48123456789")]
        [InlineData("+48abc12345")]
        [InlineData("++481234567")]
        [InlineData("+48 123 456 789!")]
        public void Constructor_WithInvalidFormat_ShouldThrowDomainException(string invalidNumber)
        {
            // Arrange & Act
            Action act = () => new PhoneNumber(invalidNumber);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage($"'{invalidNumber}' is not a valid phone number. Expected international format, e.g. +48123456789.");
        }

        [Fact]
        public void Constructor_WithExactlyMinDigits_ShouldNotThrow()
        {
            // Arrange
            var minLengthNumber = "+1234567";

            // Act
            var phoneNumber = new PhoneNumber(minLengthNumber);

            // Assert
            phoneNumber.Value.Should().Be(minLengthNumber);
        }

        [Fact]
        public void Constructor_WithOneDigitBelowMinLength_ShouldThrowDomainException()
        {
            // Arrange
            var tooShortNumber = "+123456";

            // Act
            Action act = () => new PhoneNumber(tooShortNumber);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Phone number must contain between 7 and 15 digits.");
        }

        [Fact]
        public void Constructor_WithExactlyMaxDigits_ShouldNotThrow()
        {
            // Arrange
            var maxLengthNumber = "+123456789012345";

            // Act
            var phoneNumber = new PhoneNumber(maxLengthNumber);

            // Assert
            phoneNumber.Value.Should().Be(maxLengthNumber);
        }

        [Fact]
        public void Constructor_WithOneDigitOverMaxLength_ShouldThrowDomainException()
        {
            // Arrange
            var tooLongNumber = "+1234567890123456";

            // Act
            Action act = () => new PhoneNumber(tooLongNumber);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Phone number must contain between 7 and 15 digits.");
        }

        [Fact]
        public void ToString_ShouldReturnNormalizedValue()
        {
            // Arrange
            var phoneNumber = new PhoneNumber("+48 123-456-789");

            // Act
            var result = phoneNumber.ToString();

            // Assert
            result.Should().Be("+48123456789");
        }

        [Fact]
        public void RecordEquality_WithSameNormalizedValue_ShouldReturnTrue()
        {
            // Arrange
            var phone1 = new PhoneNumber("+48 123 456 789");
            var phone2 = new PhoneNumber("+48-123-456-789");

            // Act
            var areEqual = phone1 == phone2;

            // Assert
            areEqual.Should().BeTrue();
            phone1.Should().Be(phone2);
        }

        [Fact]
        public void RecordInequality_WithDifferentValues_ShouldReturnTrue()
        {
            // Arrange
            var phone1 = new PhoneNumber("+48123456789");
            var phone2 = new PhoneNumber("+44123456789");

            // Act
            var areDifferent = phone1 != phone2;

            // Assert
            areDifferent.Should().BeTrue();
            phone1.Should().NotBe(phone2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForSameNormalizedValue()
        {
            // Arrange
            var phone1 = new PhoneNumber("+48 123 456 789");
            var phone2 = new PhoneNumber("+48-123-456-789");

            // Act
            var hashCode1 = phone1.GetHashCode();
            var hashCode2 = phone2.GetHashCode();

            // Assert
            hashCode1.Should().Be(hashCode2);
        }
    }
}