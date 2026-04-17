using AutoMapper;
using ContactList.Application.Common.Exceptions;
using ContactList.Application.Common.Mappings;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Application.Features.Contacts.Queries.GetContactById;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Contacts.Queries.GetContactById
{
    public class GetContactByIdQueryHandlerTests
    {
        private readonly Mock<IContactRepository> _contactRepo = new(MockBehavior.Strict);
        private readonly IMapper _mapper;

        public GetContactByIdQueryHandlerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ContactMappingProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ExistingContact_ReturnsDto()
        {
            // Arrange
            var contact = BuildContact("Anna", "Nowak", "anna@example.com", "+48111222333", "Prywatny", null);

            _contactRepo.Setup(r => r.GetByIdAsync(contact.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            var handler = new GetContactByIdQueryHandler(_contactRepo.Object, _mapper);
            var query = new GetContactByIdQuery(contact.Id);

            // Act
            var dto = await handler.Handle(query, CancellationToken.None);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(contact.Id);
            dto.FirstName.Should().Be("Anna");
            dto.Email.Should().Be("anna@example.com");
            dto.CategoryName.Should().Be("Prywatny");
            dto.SubcategoryName.Should().BeNull();
        }

        [Fact]
        public async Task Handle_NonExistingContact_ThrowsNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _contactRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact?)null);

            var handler = new GetContactByIdQueryHandler(_contactRepo.Object, _mapper);
            var query = new GetContactByIdQuery(id);

            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        private static Contact BuildContact(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string categoryName,
            string? subcategoryName)
        {
            var category = new Category(categoryName);
            Guid? subcategoryId = null;
            Subcategory? subcategory = null;

            if (subcategoryName is not null)
            {
                subcategory = new Subcategory(subcategoryName, category.Id);
                subcategoryId = subcategory.Id;
            }

            var contact = new Contact(
                firstName,
                lastName,
                new Email(email),
                "hash",
                new PhoneNumber(phoneNumber),