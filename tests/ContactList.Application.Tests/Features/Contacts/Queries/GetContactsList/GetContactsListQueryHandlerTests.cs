using AutoMapper;
using ContactList.Application.Common.Mappings;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Contacts.Queries.GetContactsList;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Contacts.Queries.GetContactsList
{
    public class GetContactsListQueryHandlerTests
    {
        private readonly Mock<IContactRepository> _contactRepo = new(MockBehavior.Strict);
        private readonly IMapper _mapper;

        public GetContactsListQueryHandlerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ContactMappingProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_EmptyRepository_ReturnsEmptyList()
        {
            // Arrange
            _contactRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Contact>());

            var handler = new GetContactsListQueryHandler(_contactRepo.Object, _mapper);
            var query = new GetContactsListQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ExistingContacts_ReturnsMappedDtosPreservingOrder()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                BuildContact("Anna", "Nowak", "anna@example.com", "+48111111111", "Prywatny"),
                BuildContact("Jan",  "Kowal", "jan@example.com",  "+48222222222", "Służbowy"),
                BuildContact("Ewa",  "Lis",   "ewa@example.com",  "+48333333333", "Inny"),
            };

            _contactRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(contacts);

            var handler = new GetContactsListQueryHandler(_contactRepo.Object, _mapper);
            var query = new GetContactsListQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.Select(d => d.FirstName).Should().ContainInOrder("Anna", "Jan", "Ewa");
            result.Select(d => d.CategoryName).Should().ContainInOrder("Prywatny", "Służbowy", "Inny");
            result[0].Email.Should().Be("anna@example.com");
            result[1].PhoneNumber.Should().Be("+48222222222");
        }

        private static Contact BuildContact(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string categoryName)
        {
            var category = new Category(categoryName);
            var contact = new Contact(
                firstName,
                lastName,
                new Email(email),
                "hash",
                new PhoneNumber(phoneNumber),
                new DateOnly(1990, 1, 1),
                category.Id,
                subcategoryId: null);

            typeof(Contact).GetProperty(nameof(Contact.Category))!.SetValue(contact, category);
            return contact;
        }
    }
}