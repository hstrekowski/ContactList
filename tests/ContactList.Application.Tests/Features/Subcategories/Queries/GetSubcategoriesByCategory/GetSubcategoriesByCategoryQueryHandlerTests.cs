using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Subcategories.Queries.GetSubcategoriesByCategory;
using ContactList.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Subcategories.Queries.GetSubcategoriesByCategory
{
    public class GetSubcategoriesByCategoryQueryHandlerTests
    {
        private readonly Mock<ISubcategoryRepository> _subcategoryRepo = new(MockBehavior.Strict);
        private readonly Mock<ICategoryRepository> _categoryRepo = new(MockBehavior.Strict);

        [Fact]
        public async Task Handle_CategoryHasNoSubcategories_ReturnsEmptyList()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _categoryRepo.Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _subcategoryRepo.Setup(r => r.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Subcategory>());

            var handler = new GetSubcategoriesByCategoryQueryHandler(_subcategoryRepo.Object, _categoryRepo.Object);
            var query = new GetSubcategoriesByCategoryQuery(categoryId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ExistingSubcategories_ReturnsMappedDtosPreservingData()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var subcategories = new[]
            {
                new Subcategory("Szef", categoryId),
                new Subcategory("Klient", categoryId),
                new Subcategory("Współpracownik", categoryId),
            };

            _categoryRepo.Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _subcategoryRepo.Setup(r => r.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subcategories);

            var handler = new GetSubcategoriesByCategoryQueryHandler(_subcategoryRepo.Object, _categoryRepo.Object);
            var query = new GetSubcategoriesByCategoryQuery(categoryId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.Select(d => d.Name).Should().ContainInOrder("Szef", "Klient", "Współpracownik");
            result.Should().OnlyContain(d => d.CategoryId == categoryId);
            result[0].Id.Should().Be(subcategories[0].Id);
        }
    }
}