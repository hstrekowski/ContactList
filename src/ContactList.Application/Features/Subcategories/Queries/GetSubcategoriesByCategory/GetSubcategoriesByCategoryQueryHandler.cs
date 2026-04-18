using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Subcategories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Subcategories.Queries.GetSubcategoriesByCategory
{
    /// <summary>
    /// Retrieves subcategories for a specific category and maps them to DTOs. 
    /// Returns an empty list if the category ID is invalid or if the category (like 'Prywatny') simply has no subcategories.
    /// </summary>
    public sealed class GetSubcategoriesByCategoryQueryHandler
        : IRequestHandler<GetSubcategoriesByCategoryQuery, IReadOnlyList<SubcategoryDto>>
    {
        private readonly ISubcategoryRepository _subcategoryRepository;

        public GetSubcategoriesByCategoryQueryHandler(ISubcategoryRepository subcategoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
        }

        public async Task<IReadOnlyList<SubcategoryDto>> Handle(
            GetSubcategoriesByCategoryQuery request,
            CancellationToken cancellationToken)
        {
            var subcategories = await _subcategoryRepository.GetByCategoryIdAsync(request.CategoryId, cancellationToken);
            return subcategories.Select(s => new SubcategoryDto(s.Id, s.Name, s.CategoryId)).ToList();
        }
    }
}
