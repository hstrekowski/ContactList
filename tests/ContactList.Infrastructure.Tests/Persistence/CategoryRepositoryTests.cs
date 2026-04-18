using ContactList.Domain.Entities;
using ContactList.Infrastructure.Persistence;
using ContactList.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ContactList.Infrastructure.Tests.Persistence.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryRepository _sut;

    public CategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new CategoryRepository(_dbContext);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCategoriesWithSubcategories_OrderedByName()
    {
        // Arrange
        var categoryB = new Category("B Category");
        var categoryA = new Category("A Category");

        await _dbContext.Categories.AddRangeAsync(categoryB, categoryA);
        await _dbContext.SaveChangesAsync();

        var subcategoryForB = new Subcategory("Sub B", categoryB.Id);
        _dbContext.Add(subcategoryForB);
        await _dbContext.SaveChangesAsync();

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("A Category");
        result[1].Name.Should().Be("B Category");
        result[1].Subcategories.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategoryWithSubcategories_WhenCategoryExists()
    {
        // Arrange
        var category = new Category("Test Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var subcategory = new Subcategory("Test Sub", category.Id);
        _dbContext.Add(subcategory);
        await _dbContext.SaveChangesAsync();

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetByIdAsync(category.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
        result.Name.Should().Be("Test Category");
        result.Subcategories.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        // Arrange & Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}