using ContactList.Application.Features.Categories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Categories.Queries.GetCategories
{
    /// <summary>
    /// Returns all categories as a flat list for dropdown menus.
    /// </summary>
    public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
}
