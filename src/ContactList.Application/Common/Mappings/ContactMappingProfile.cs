using AutoMapper;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Domain.Entities;

namespace ContactList.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile that projects the <see cref="Contact"/> aggregate to the
    /// read-only DTOs returned from the query handlers. Flattens value objects
    /// (<c>Email</c>, <c>PhoneNumber</c>) and pulls display names from the related
    /// <c>Category</c> / <c>Subcategory</c> navigation properties.
    /// Callers must ensure EF has eagerly loaded those navigations.
    /// </summary>
    public sealed class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<Contact, ContactListItemDto>()
                .ConvertUsing(src => new ContactListItemDto(
                    src.Id,
                    src.FirstName,
                    src.LastName,
                    src.Email.Value,
                    src.PhoneNumber.Value,
                    src.Category.Name));

            CreateMap<Contact, ContactDetailDto>()
                .ConvertUsing(src => new ContactDetailDto(
                    src.Id,
                    src.FirstName,
                    src.LastName,
                    src.Email.Value,
                    src.PhoneNumber.Value,
                    src.DateOfBirth,
                    src.CategoryId,
                    src.Category.Name,
                    src.SubcategoryId,
                    src.Subcategory != null ? src.Subcategory.Name : null));
        }
    }
}
