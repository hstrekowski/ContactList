using ContactList.Application.Features.Categories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Categories.Queries.GetCategories
{
    /// <summary>
    /// Returns every seeded category as a flat list. Used to populate the category
    /// dropdown on the contact form.
    /// </summary>
    public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
}
