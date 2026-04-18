using ContactList.Domain.Entities;
using ContactList.Infrastructure.Persistence;
using ContactList.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Tests.Persistence.Repositories;

public class SubcategoryRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SubcategoryRepository _sut;

    public SubcategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new SubcategoryRepository(_dbContext);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ShouldReturnSubcategories_OrderedByName()
    {
        // Arrange
        var targetCategoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();

        var subcategoryB = new Subcategory("B Test Sub 1", targetCategoryId);
        var subcategoryA = new Subcategory("A Test Sub 2", targetCategoryId);
        var otherSubcategory = new Subcategory("Test Sub 3", otherCategoryId);

        await _dbContext.Subcategories.AddRangeAsync(subcategoryB, subcategoryA, otherSubcategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByCategoryIdAsync(targetCategoryId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("A Test Sub 2");
        result[1].Name.Should().Be("B Test Sub 1");
    }

    [Fact]
    public async Task ExistsInCategoryAsync_ShouldReturnTrue_WhenSubcategoryExistsInGivenCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var subcategory = new Subcategory("Test Sub", categoryId);

        await _dbContext.Subcategories.AddAsync(subcategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsInCategoryAsync(subcategory.Id, categoryId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsInCategoryAsync_ShouldReturnFalse_WhenSubcategoryBelongsToDifferentCategory()
    {
        // Arrange
        var correctCategoryId = Guid.NewGuid();
        var wrongCategoryId = Guid.NewGuid();
        var subcategory = new Subcategory("Test Sub", correctCategoryId);

        await _dbContext.Subcategories.AddAsync(subcategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsInCategoryAsync(subcategory.Id, wrongCategoryId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsInCategoryAsync_ShouldReturnFalse_WhenSubcategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var nonExistentSubcategoryId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsInCategoryAsync(nonExistentSubcategoryId, categoryId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_ShouldAddSubcategoryToDbContext()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var subcategory = new Subcategory("New Subcategory", categoryId);

        // Act
        await _sut.AddAsync(subcategory, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert
        var addedEntity = await _dbContext.Subcategories.FindAsync(subcategory.Id);
        addedEntity.Should().NotBeNull();
        addedEntity!.Name.Should().Be("New Subcategory");
        addedEntity.CategoryId.Should().Be(categoryId);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}