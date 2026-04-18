using AutoMapper;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Domain.Entities;

namespace ContactList.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for contacts. Flattens value objects and maps category names. Make sure to Include() navigations in EF before using this.
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
