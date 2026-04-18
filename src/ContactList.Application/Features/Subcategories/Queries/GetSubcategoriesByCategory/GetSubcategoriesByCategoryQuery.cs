using ContactList.Application.Features.Subcategories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Subcategories.Queries.GetSubcategoriesByCategory
{
    /// <summary>
    /// Fetches all subcategories for a specific category. This is used to populate the dependent subcategory dropdown in the UI.
    /// </summary>
    /// <param name="CategoryId">Identifier of the parent category.</param>
    public sealed record GetSubcategoriesByCategoryQuery(Guid CategoryId) : IRequest<IReadOnlyList<SubcategoryDto>>;
}
