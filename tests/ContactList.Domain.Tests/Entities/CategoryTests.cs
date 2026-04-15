using FluentAssertions;
using ContactList.Domain.Entities;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Tests.Entities
{
    public class CategoryTests
    {
        private class TestCategory : Category
        {
            public TestCategory() : base() { }
        }

        [Theory]
        [InlineData("Work", "Work")]
        [InlineData("  Family  ", "Family")]
        [InlineData("Friends", "Friends")]
        public void Constructor_WithValidName_ShouldCreateInstanceAndTrimName(string input, string expectedName)
        {
            // Arrange & Act
            var category = new Category(input);

            // Assert
            category.Name.Should().Be(expectedName);
            category.Subcategories.Should().NotBeNull().And.BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyName_ShouldThrowDomainException(string invalidName)
        {
            // Arrange & Act
            Action act = () => new Category(invalidName);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Category name cannot be empty.");
        }

        [Fact]
        public void ProtectedConstructor_ShouldInitializePropertiesWithDefaultValues()
        {
            // Arrange & Act
            var category = new TestCategory();

            // Assert
            category.Name.Should().BeEmpty();
            category.Subcategories.Should().NotBeNull().And.BeEmpty();
        }
    }
}