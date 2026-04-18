using AutoMapper;
using ContactList.Application.Common.Mappings;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentAssertions;

namespace ContactList.Application.Tests.Common.Mappings
{
    public class ContactMappingProfileTests
    {
        private readonly IMapper _mapper;

        public ContactMappingProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ContactMappingProfile>());
            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void AssertConfigurationIsValid_ContactMappingProfile_DoesNotThrow()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ContactMappingProfile>());
            var act = () => config.AssertConfigurationIsValid();

            // Act & Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Map_ContactToContactListItemDto_ReturnsMappedDto()
        {
            // Arrange
            var contact = BuildContact(
                "Jan",
                "Kowalski",
                "jan.kowalski@example.com",
                "+48123456789",
                categoryName: "Służbowy",
                subcategoryName: "Szef");

            // Act
            var dto = _mapper.Map<ContactListItemDto>(contact);

            // Assert
            dto.Id.Should().Be(contact.Id);
            dto.FirstName.Should().Be("Jan");
            dto.LastName.Should().Be("Kowalski");
            dto.CategoryName.Should().Be("Służbowy");
        }

        [Fact]
        public void Map_ContactToContactDetailDtoWithSubcategory_ReturnsMappedDto()
        {
            // Arrange
            var contact = BuildContact(
                "Anna",
                "Nowak",
                "anna@example.com",
                "+48987654321",
                categoryName: "Służbowy",
                subcategoryName: "Klient");

            // Act
            var dto = _mapper.Map<ContactDetailDto>(contact);

            // Assert
            dto.Id.Should().Be(contact.Id);
            dto.FirstName.Should().Be("Anna");
            dto.LastName.Should().Be("Nowak");
            dto.Email.Should().Be("anna@example.com");
            dto.PhoneNumber.Should().Be("+48987654321");
            dto.CategoryId.Should().Be(contact.CategoryId);
            dto.CategoryName.Should().Be("Służbowy");
            dto.SubcategoryId.Should().Be(contact.SubcategoryId);
            dto.SubcategoryName.Should().Be("Klient");
        }

        [Fact]
        public void Map_ContactToContactDetailDtoWithoutSubcategory_ReturnsMappedDto()
        {
            // Arrange
            var contact = BuildContact(
                "Marek",
                "Wiśniewski",
                "marek@example.com",
                "+48111222333",
                categoryName: "Prywatny",
                subcategoryName: null);

            // Act
            var dto = _mapper.Map<ContactDetailDto>(contact);

            // Assert
            dto.SubcategoryId.Should().BeNull();
            dto.SubcategoryName.Should().BeNull();
            dto.CategoryName.Should().Be("Prywatny");
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
                new DateOnly(1990, 1, 1),
                category.Id,
                subcategoryId);

            SetPrivateProperty(contact, nameof(Contact.Category), category);
            if (subcategory is not null)
                SetPrivateProperty(contact, nameof(Contact.Subcategory), subcategory);

            return contact;
        }

        private static void SetPrivateProperty(object target, string name, object value)
        {
            var prop = target.GetType().GetProperty(name)
                ?? throw new InvalidOperationException($"Property {name} not found.");
            prop.SetValue(target, value);
        }
    }
}