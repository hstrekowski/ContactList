using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using ContactList.Infrastructure.Persistence;
using ContactList.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Tests.Persistence.Repositories;

public class ContactRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ContactRepository _sut;

    public ContactRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new ContactRepository(_dbContext);
    }

    private Email CreateTestEmail(string address)
    {
        return new Email(address);
    }

    private PhoneNumber CreateTestPhoneNumber(string number = "+48123456789")
    {
        return new PhoneNumber(number);
    }

    private Contact CreateTestContact(Guid categoryId, Guid? subcategoryId, string firstName, string lastName, string emailAddress)
    {
        return new Contact(
            firstName,
            lastName,
            CreateTestEmail(emailAddress),
            "hashed_password",
            CreateTestPhoneNumber(),
            new DateOnly(1990, 1, 1),
            categoryId,
            subcategoryId
        );
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnContactWithCategoryAndSubcategory_WhenExists()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);

        var subcategory = new Subcategory("Subcategory", category.Id);
        await _dbContext.Subcategories.AddAsync(subcategory);
        await _dbContext.SaveChangesAsync();

        var contact = CreateTestContact(category.Id, subcategory.Id, "John", "Doe", "test@test.com");
        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetByIdAsync(contact.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contact.Id);
        result.Category.Should().NotBeNull();
        result.Subcategory.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnContacts_OrderedByLastNameThenFirstName()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var contact1 = CreateTestContact(category.Id, null, "John", "Zimmerman", "john@test.com");
        var contact2 = CreateTestContact(category.Id, null, "Alice", "Adams", "alice@test.com");
        var contact3 = CreateTestContact(category.Id, null, "Bob", "Adams", "bob@test.com");

        await _dbContext.Contacts.AddRangeAsync(contact1, contact2, contact3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].FirstName.Should().Be("Alice");
        result[1].FirstName.Should().Be("Bob");
        result[2].FirstName.Should().Be("John");
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var contact = CreateTestContact(category.Id, null, "Test", "Test", "existing@test.com");
        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();

        var emailToCheck = CreateTestEmail("existing@test.com");

        // Act
        var result = await _sut.ExistsByEmailAsync(emailToCheck, null, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailExistsButIsExcluded()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var contact = CreateTestContact(category.Id, null, "Test", "Test", "existing@test.com");
        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();

        var emailToCheck = CreateTestEmail("existing@test.com");

        // Act
        var result = await _sut.ExistsByEmailAsync(emailToCheck, contact.Id, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Arrange
        var emailToCheck = CreateTestEmail("nonexistent@test.com");

        // Act
        var result = await _sut.ExistsByEmailAsync(emailToCheck, null, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailExistsOnDifferentContactExcludingSelf()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var otherContact = CreateTestContact(category.Id, null, "Other", "User", "taken@test.com");
        var currentContact = CreateTestContact(category.Id, null, "Current", "User", "mine@test.com");
        await _dbContext.Contacts.AddRangeAsync(otherContact, currentContact);
        await _dbContext.SaveChangesAsync();

        var emailToCheck = CreateTestEmail("taken@test.com");

        // Act
        var result = await _sut.ExistsByEmailAsync(emailToCheck, currentContact.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_ShouldAddContactToDbContext()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var contact = CreateTestContact(category.Id, null, "New", "User", "new@test.com");

        // Act
        await _sut.AddAsync(contact, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert
        var addedEntity = await _dbContext.Contacts.FindAsync(contact.Id);
        addedEntity.Should().NotBeNull();
        addedEntity!.FirstName.Should().Be("New");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateContactInDbContext()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var contact = CreateTestContact(category.Id, null, "Old", "Name", "test@test.com");
        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();

        contact.Update("Updated", "Name", contact.Email, contact.PhoneNumber, contact.DateOfBirth, category.Id, null);

        // Act
        await _sut.UpdateAsync(contact, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedEntity = await _dbContext.Contacts.FindAsync(contact.Id);
        updatedEntity!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveContactFromDbContext()
    {
        // Arrange
        var category = new Category("Category");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var contact = CreateTestContact(category.Id, null, "To", "Delete", "delete@test.com");
        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.DeleteAsync(contact, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedEntity = await _dbContext.Contacts.FindAsync(contact.Id);
        deletedEntity.Should().BeNull();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}