using FluentAssertions;
using ContactList.Domain.Entities;
using ContactList.Domain.Exceptions;
using ContactList.Domain.ValueObjects;

namespace ContactList.Domain.Tests.Entities
{
    public class ContactTests
    {
        private class TestContact : Contact
        {
            public TestContact() : base() { }
        }

        private readonly Email _validEmail = new("test@example.com");
        private readonly PhoneNumber _validPhone = new("+48123456789");
        private readonly string _validPasswordHash = "hashed_password_123";
        private readonly Guid _validCategoryId = Guid.NewGuid();
        private readonly Guid _validSubcategoryId = Guid.NewGuid();
        private readonly DateOnly _validDateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

        [Fact]
        public void Constructor_WithValidData_ShouldCreateInstanceAndSetProperties()
        {
            // Arrange
            var firstName = "  John  ";
            var lastName = " Doe ";

            // Act
            var contact = new Contact(
                firstName,
                lastName,
                _validEmail,
                _validPasswordHash,
                _validPhone,
                _validDateOfBirth,
                _validCategoryId,
                _validSubcategoryId);

            // Assert
            contact.FirstName.Should().Be("John");
            contact.LastName.Should().Be("Doe");
            contact.Email.Should().Be(_validEmail);
            contact.PasswordHash.Should().Be(_validPasswordHash);
            contact.PhoneNumber.Should().Be(_validPhone);
            contact.DateOfBirth.Should().Be(_validDateOfBirth);
            contact.CategoryId.Should().Be(_validCategoryId);
            contact.SubcategoryId.Should().Be(_validSubcategoryId);
            contact.Category.Should().BeNull();
            contact.Subcategory.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithNullSubcategoryId_ShouldCreateInstance()
        {
            // Arrange & Act
            var contact = new Contact(
                "John",
                "Doe",
                _validEmail,
                _validPasswordHash,
                _validPhone,
                _validDateOfBirth,
                _validCategoryId,
                null);

            // Assert
            contact.SubcategoryId.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidFirstName_ShouldThrowDomainException(string? invalidFirstName)
        {
            // Arrange & Act
            Action act = () => new Contact(
                invalidFirstName!, "Doe", _validEmail, _validPasswordHash, _validPhone, _validDateOfBirth, _validCategoryId, null);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("First name cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidLastName_ShouldThrowDomainException(string? invalidLastName)
        {
            // Arrange & Act
            Action act = () => new Contact(
                "John", invalidLastName!, _validEmail, _validPasswordHash, _validPhone, _validDateOfBirth, _validCategoryId, null);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Last name cannot be empty.");
        }

        [Fact]
        public void Constructor_WithNullEmail_ShouldThrowDomainException()
        {
            // Arrange & Act
            Action act = () => new Contact(
                "John", "Doe", null!, _validPasswordHash, _validPhone, _validDateOfBirth, _validCategoryId, null);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Email is required.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidPasswordHash_ShouldThrowDomainException(string? invalidPasswordHash)
        {
            // Arrange & Act
            Action act = () => new Contact(
                "John", "Doe", _validEmail, invalidPasswordHash!, _validPhone, _validDateOfBirth, _validCategoryId, null);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Password hash is required.");
        }

        [Fact]
        public void Constructor_WithNullPhoneNumber_ShouldThrowDomainException()
        {
            // Arrange & Act
            Action act = () => new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, null!, _validDateOfBirth, _validCategoryId, null);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Phone number is required.");
        }

        [Fact]
        public void Constructor_WithFutureDateOfBirth_ShouldThrowDomainException()
        {
            // Arrange
            var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            // Act
            Action act = () => new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, _validPhone, futureDate, _validCategoryId, null);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Date of birth cannot be in the future.");
        }

        [Fact]
        public void Constructor_WithTodayDateOfBirth_ShouldNotThrow()
        {
            // Arrange
            var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var contact = new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, _validPhone, todayDate, _validCategoryId, null);

            // Assert
            contact.DateOfBirth.Should().Be(todayDate);
        }

        [Fact]
        public void Constructor_WithEmptyCategoryId_ShouldThrowDomainException()
        {
            // Arrange & Act
            Action act = () => new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, _validPhone, _validDateOfBirth, Guid.Empty, null);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Contact must be assigned to a valid category.");
        }

        [Fact]
        public void Constructor_WithEmptySubcategoryId_ShouldThrowDomainException()
        {
            // Arrange & Act
            Action act = () => new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, _validPhone, _validDateOfBirth, _validCategoryId, Guid.Empty);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Subcategory id must be a valid Guid.");
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateProperties()
        {
            // Arrange
            var contact = new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, _validPhone, _validDateOfBirth, _validCategoryId, null);

            var newEmail = new Email("new@example.com");
            var newPhone = new PhoneNumber("+1234567890");
            var newDateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));
            var newCategoryId = Guid.NewGuid();
            var newSubcategoryId = Guid.NewGuid();

            // Act
            contact.Update(
                "  Jane  ",
                " Smith ",
                newEmail,
                newPhone,
                newDateOfBirth,
                newCategoryId,
                newSubcategoryId);

            // Assert
            contact.FirstName.Should().Be("Jane");
            contact.LastName.Should().Be("Smith");
            contact.Email.Should().Be(newEmail);
            contact.PhoneNumber.Should().Be(newPhone);
            contact.DateOfBirth.Should().Be(newDateOfBirth);
            contact.CategoryId.Should().Be(newCategoryId);
            contact.SubcategoryId.Should().Be(newSubcategoryId);
            contact.PasswordHash.Should().Be(_validPasswordHash);
        }

        [Fact]
        public void ChangePasswordHash_WithValidHash_ShouldUpdatePasswordHash()
        {
            // Arrange
            var contact = new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, _validPhone, _validDateOfBirth, _validCategoryId, null);
            var newHash = "new_secure_hash_456";

            // Act
            contact.ChangePasswordHash(newHash);

            // Assert
            contact.PasswordHash.Should().Be(newHash);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangePasswordHash_WithInvalidHash_ShouldThrowDomainException(string? invalidHash)
        {
            // Arrange
            var contact = new Contact(
                "John", "Doe", _validEmail, _validPasswordHash, _validPhone, _validDateOfBirth, _validCategoryId, null);

            // Act
            Action act = () => contact.ChangePasswordHash(invalidHash!);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Password hash is required.");
        }

        [Fact]
        public void ProtectedConstructor_ShouldInitializePropertiesWithDefaultValues()
        {
            // Arrange & Act
            var contact = new TestContact();

            // Assert
            contact.FirstName.Should().BeEmpty();
            contact.LastName.Should().BeEmpty();
            contact.Email.Should().BeNull();
            contact.PasswordHash.Should().BeEmpty();
            contact.PhoneNumber.Should().BeNull();
            contact.DateOfBirth.Should().Be(default);
            contact.CategoryId.Should().Be(Guid.Empty);
            contact.Category.Should().BeNull();
            contact.SubcategoryId.Should().BeNull();
            contact.Subcategory.Should().BeNull();
        }
    }
}