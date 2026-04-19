using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Contacts.Commands.DeleteContact;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Contacts.Commands.DeleteContact
{
    public class DeleteContactCommandHandlerTests
    {
        private readonly Mock<IContactRepository> _contactRepo = new(MockBehavior.Strict);
        private readonly Mock<ISubcategoryRepository> _subcategoryRepo = new(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);

        private DeleteContactCommandHandler CreateHandler() =>
            new(_contactRepo.Object, _subcategoryRepo.Object, _uow.Object);

        private static Contact ExistingContact(string categoryName = "Prywatny")
        {
            var category = new Category(categoryName);
            var contact = new Contact(
                "Jan",
                "Kowalski",
                new Email("jan@example.com"),
                "hash",
                new PhoneNumber("+48123456789"),
                new DateOnly(1990, 1, 1),
                category.Id,
                subcategoryId: null);

            typeof(Contact)
                .GetProperty(nameof(Contact.Category))!
                .SetValue(contact, category);

            return contact;
        }

        [Fact]
        public async Task Handle_ExistingContact_DeletesAndCommits()
        {
            // Arrange
            var existing = ExistingContact();
            var cmd = new DeleteContactCommand(existing.Id);

            _contactRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);
            _contactRepo.Setup(r => r.DeleteAsync(existing, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await CreateHandler().Handle(cmd, CancellationToken.None);

            // Assert
            _contactRepo.Verify(r => r.DeleteAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistingContact_ThrowsNotFoundException()
        {
            // Arrange
            var cmd = new DeleteContactCommand(Guid.NewGuid());
            _contactRepo.Setup(r => r.GetByIdAsync(cmd.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact?)null);

            var act = async () => await CreateHandler().Handle(cmd, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _contactRepo.Verify(r => r.DeleteAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}