using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Contracts.Security;
using ContactList.Application.Features.Contacts.Commands.CreateContact;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Contacts.Commands.CreateContact
{
    public class CreateContactCommandHandlerTests
    {
        private readonly Mock<IContactRepository> _contactRepo = new(MockBehavior.Strict);
        private readonly Mock<ICategoryRepository> _categoryRepo = new(MockBehavior.Strict);
        private readonly Mock<ISubcategoryRepository> _subcategoryRepo = new(MockBehavior.Strict);
        private readonly Mock<IPasswordHasher> _hasher = new(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);

        private CreateContactCommandHandler CreateHandler() =>
            new(_contactRepo.Object, _categoryRepo.Object, _subcategoryRepo.Object, _hasher.Object, _uow.Object);

        private static CreateContactCommand BaseCommand(Guid categoryId, Guid? subcategoryId = null, string? subcategoryName = null) =>
            new(
                FirstName: "Jan",
                LastName: "Kowalski",
                Email: "jan@example.com",
                Password: "Password1!",
                PhoneNumber: "+48123456789",
                DateOfBirth: new DateOnly(1990, 1, 1),
                CategoryId: categoryId,
                SubcategoryId: subcategoryId,
                SubcategoryName: subcategoryName);

        [Fact]
        public async Task Handle_PrywatnyCategoryWithoutSubcategory_CreatesContact()
        {
            // Arrange
            var category = new Category("Prywatny");
            var cmd = BaseCommand(category.Id);

            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _hasher.Setup(h => h.Hash("Password1!")).Returns("hashed");
            _contactRepo.Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var id = await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            id.Should().NotBeEmpty();
            _contactRepo.Verify(r => r.AddAsync(
                It.Is<Contact>(c => c.SubcategoryId == null && c.PasswordHash == "hashed"),
                It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_SluzbowyCategoryWithExistingSubcategory_CreatesContact()
        {
            // Arrange
            var category = new Category("Służbowy");
            var subcategoryId = Guid.NewGuid();
            var cmd = BaseCommand(category.Id, subcategoryId: subcategoryId);

            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _subcategoryRepo.Setup(r => r.ExistsInCategoryAsync(subcategoryId, category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");
            _contactRepo.Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            _contactRepo.Verify(r => r.AddAsync(
                It.Is<Contact>(c => c.SubcategoryId == subcategoryId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InnyCategoryWithCustomSubcategory_AddsSubcategoryAndCreatesContact()
        {
            // Arrange
            var category = new Category("Inny");
            var cmd = BaseCommand(category.Id, subcategoryName: "Znajomy");

            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _subcategoryRepo.Setup(r => r.AddAsync(It.IsAny<Subcategory>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");
            _contactRepo.Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            _subcategoryRepo.Verify(r => r.AddAsync(
                It.Is<Subcategory>(s => s.Name == "Znajomy" && s.CategoryId == category.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmailAlreadyExists_ThrowsConflictException()
        {
            // Arrange
            var cmd = BaseCommand(Guid.NewGuid());
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ConflictException>();
            _categoryRepo.VerifyNoOtherCalls();
            _uow.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var cmd = BaseCommand(Guid.NewGuid());
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
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
            var cmd = BaseCommand(category.Id, subcategoryId: null);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_SluzbowyHasExtraSubcategoryName_ThrowsValidationException()
        {
            // Arrange
            var category = new Category("Służbowy");
            var cmd = BaseCommand(category.Id, subcategoryId: Guid.NewGuid(), subcategoryName: "Szef");
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_SluzbowySubcategoryNotInCategory_ThrowsNotFoundException()
        {
            // Arrange
            var category = new Category("Służbowy");
            var subcategoryId = Guid.NewGuid();
            var cmd = BaseCommand(category.Id, subcategoryId: subcategoryId);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _subcategoryRepo.Setup(r => r.ExistsInCategoryAsync(subcategoryId, category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_InnyMissingSubcategoryName_ThrowsValidationException()
        {
            // Arrange
            var category = new Category("Inny");
            var cmd = BaseCommand(category.Id, subcategoryName: null);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_InnyHasSubcategoryId_ThrowsValidationException()
        {
            // Arrange
            var category = new Category("Inny");
            var cmd = BaseCommand(category.Id, subcategoryId: Guid.NewGuid(), subcategoryName: "x");
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_PrywatnyHasSubcategory_ThrowsValidationException()
        {
            // Arrange
            var category = new Category("Prywatny");
            var cmd = BaseCommand(category.Id, subcategoryId: Guid.NewGuid());
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_UnknownCategoryName_ThrowsValidationException()
        {
            // Arrange
            var category = new Category("Unknown");
            var cmd = BaseCommand(category.Id);
            _contactRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}