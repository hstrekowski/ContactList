using FluentAssertions;
using ContactList.Domain.Entities;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Tests.Entities
{
    public class SubcategoryTests
    {
        private class TestSubcategory : Subcategory
        {
            public TestSubcategory() : base() { }
        }

        [Theory]
        [InlineData("Gaming", "Gaming")]
        [InlineData("  Sports  ", "Sports")]
        [InlineData("Tech", "Tech")]
        public void Constructor_WithValidData_ShouldCreateInstanceAndSetProperties(string inputName, string expectedName)
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var subcategory = new Subcategory(inputName, categoryId);

            // Assert
            subcategory.Name.Should().Be(expectedName);
            subcategory.CategoryId.Should().Be(categoryId);
        }

        [Fact]
        public void Constructor_WithEmptyCategoryId_ShouldThrowDomainException()
        {
            // Arrange & Act
            Action act = () => new Subcategory("Valid Name", Guid.Empty);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Category ID cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyName_ShouldThrowDomainException(string? invalidName)
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            Action act = () => new Subcategory(invalidName!, categoryId);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Subcategory name cannot be empty.");
        }

        [Theory]
        [InlineData("New Name", "New Name")]
        [InlineData("  Spaced Name  ", "Spaced Name")]
        public void Rename_WithValidName_ShouldUpdateAndTrimName(string newName, string expectedName)
        {
            // Arrange
            var subcategory = new Subcategory("Old Name", Guid.NewGuid());

            // Act
            subcategory.Rename(newName);

            // Assert
            subcategory.Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Rename_WithEmptyName_ShouldThrowDomainException(string? invalidName)
        {
            // Arrange
            var subcategory = new Subcategory("Old Name", Guid.NewGuid());

            // Act
            Action act = () => subcategory.Rename(invalidName!);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Subcategory name cannot be empty.");
        }

        [Fact]
        public void ProtectedConstructor_ShouldInitializePropertiesWithDefaultValues()
        {
            // Arrange & Act
            var subcategory = new TestSubcategory();

            // Assert
            subcategory.Name.Should().BeEmpty();
            subcategory.CategoryId.Should().Be(Guid.Empty);
            subcategory.Category.Should().BeNull();
        }
    }
}