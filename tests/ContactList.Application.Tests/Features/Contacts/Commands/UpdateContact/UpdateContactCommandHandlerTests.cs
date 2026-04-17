using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Contracts.Security;
using ContactList.Application.Features.Contacts.Commands.UpdateContact;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Contacts.Commands.UpdateContact
{
    public class UpdateContactCommandHandlerTests
    {
        private readonly Mock<IContactRepository> _contactRepo = new(MockBehavior.Strict);
        private readonly Mock<ICategoryRepository> _categoryRepo = new(MockBehavior.Strict);
        private readonly Mock<ISubcategoryRepository> _subcategoryRepo = new(MockBehavior.Strict);
        private readonly Mock<IPasswordHasher> _hasher = new(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);

        private UpdateContactCommandHandler CreateHandler() =>
            new(_contactRepo.Object, _categoryRepo.Object, _subcategoryRepo.Object, _hasher.Object, _uow.Object);

        private static UpdateContactCommand BaseCommand(
            Guid id,
            Guid categoryId,
            Guid? subcategoryId = null,
            string? subcategoryName = null,
            string? password = null) =>
            new(
                Id: id,
                FirstName: "Updated",
                LastName: "Name",
                Email: "new@example.com",
                Password: password,
                PhoneNumber: "+48999888777",
                DateOfBirth: new DateOnly(1985, 5, 15),
                CategoryId: categoryId,
                SubcategoryId: subcategoryId,
                SubcategoryName: subcategoryName);

        private static Contact ExistingContact(Guid categoryId) =>
            new(
                "Old",
                "Person",
                new Email("old@example.com"),
                "old-hash",
                new PhoneNumber("+48000000000"),
                new DateOnly(1980, 1, 1),
                categoryId,
                subcategoryId: null);

        [Fact]
        public async Task Handle_PrywatnyCategoryWithNullPassword_UpdatesContactWithoutChangingPassword()
        {
            // Arrange
            var category = new Category("Prywatny");
            var existing = ExistingContact(category.Id);
            var cmd = BaseCommand(existing.Id, category.Id, password: null);

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _contactRepo.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            existing.FirstName.Should().Be("Updated");
            existing.Email.Value.Should().Be("new@example.com");
            existing.PasswordHash.Should().Be("old-hash");
            _hasher.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NewPasswordSupplied_UpdatesContactAndRehashesPassword()
        {
            // Arrange
            var category = new Category("Prywatny");
            var existing = ExistingContact(category.Id);
            var cmd = BaseCommand(existing.Id, category.Id, password: "NewPass1!");

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _hasher.Setup(h => h.Hash("NewPass1!")).Returns("new-hash");
            _contactRepo.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            existing.PasswordHash.Should().Be("new-hash");
            _hasher.Verify(h => h.Hash("NewPass1!"), Times.Once);
        }

        [Fact]
        public async Task Handle_SluzbowyCategoryWithExistingSubcategory_UpdatesContact()
        {
            // Arrange
            var category = new Category("Służbowy");
            var existing = ExistingContact(category.Id);
            var subcategoryId = Guid.NewGuid();
            var cmd = BaseCommand(existing.Id, category.Id, subcategoryId: subcategoryId);

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _subcategoryRepo.Setup(r => r.ExistsInCategoryAsync(subcategoryId, category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _contactRepo.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            existing.SubcategoryId.Should().Be(subcategoryId);
        }

        [Fact]
        public async Task Handle_InnyCategory_AddsNewSubcategoryAndUpdatesContact()
        {
            // Arrange
            var category = new Category("Inny");
            var existing = ExistingContact(category.Id);
            var cmd = BaseCommand(existing.Id, category.Id, subcategoryName: "Sąsiad");

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _subcategoryRepo.Setup(r => r.AddAsync(It.IsAny<Subcategory>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _contactRepo.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            _subcategoryRepo.Verify(r => r.AddAsync(
                It.Is<Subcategory>(s => s.Name == "Sąsiad" && s.CategoryId == category.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ContactDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var cmd = BaseCommand(Guid.NewGuid(), Guid.NewGuid());
            _contactRepo.Setup(r => r.GetByIdAsync(cmd.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact?)null);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_EmailTakenByAnotherContact_ThrowsConflictException()
        {
            // Arrange
            var category = new Category("Prywatny");
            var existing = ExistingContact(category.Id);
            var cmd = BaseCommand(existing.Id, category.Id);

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var existing = ExistingContact(Guid.NewGuid());
            var cmd = BaseCommand(existing.Id, Guid.NewGuid());

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(cmd.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_SluzbowyMissingSubcategoryId_ThrowsValidationException()
        {
            // Arrange
            var category = new Category("Służbowy");
            var existing = ExistingContact(category.Id);
            var cmd = BaseCommand(existing.Id, category.Id, subcategoryId: null);

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_PrywatnyHasSubcategory_ThrowsValidationException()
        {
            // Arrange
            var category = new Category("Prywatny");
            var existing = ExistingContact(category.Id);
            var cmd = BaseCommand(existing.Id, category.Id, subcategoryId: Guid.NewGuid());

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}