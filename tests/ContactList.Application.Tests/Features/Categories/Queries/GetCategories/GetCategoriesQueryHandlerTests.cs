using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Categories.Queries.GetCategories;
using ContactList.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _repo = new(MockBehavior.Strict);

        [Fact]
        public async Task Handle_EmptyRepository_ReturnsEmptyList()
        {
            // Arrange
            _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Category>());

            var handler = new GetCategoriesQueryHandler(_repo.Object);
            var query = new GetCategoriesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ExistingCategories_ReturnsMappedDtos()
        {
            // Arrange
            var categories = new[]
            {
                new Category("Służbowy"),
                new Category("Prywatny"),
                new Category("Inny"),
            };

            _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            var handler = new GetCategoriesQueryHandler(_repo.Object);
            var query = new GetCategoriesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.Select(d => d.Name).Should().ContainInOrder("Służbowy", "Prywatny", "Inny");
            result[0].Id.Should().Be(categories[0].Id);
            result[1].Id.Should().Be(categories[1].Id);
            result[2].Id.Should().Be(categories[2].Id);
        }
    }
}