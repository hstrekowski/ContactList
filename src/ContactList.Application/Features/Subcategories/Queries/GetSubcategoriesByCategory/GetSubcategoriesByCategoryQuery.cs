using ContactList.Application.Features.Subcategories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Subcategories.Queries.GetSubcategoriesByCategory
{
    /// <summary>
    /// Returns every subcategory belonging to the given category. The frontend calls
    /// this after the user picks a category in the contact form so the second
    /// dropdown can show the matching subcategories.
    /// </summary>
    /// <param name="CategoryId">Identifier of the parent category.</param>
    public sealed record GetSubcategoriesByCategoryQuery(Guid CategoryId) : IRequest<IReadOnlyList<SubcategoryDto>>;
}
