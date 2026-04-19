using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Subcategories.Queries.DTOs;
using ContactList.Domain.Entities;
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
        private readonly ICategoryRepository _categoryRepository;

        public GetSubcategoriesByCategoryQueryHandler(ISubcategoryRepository subcategoryRepository, ICategoryRepository categoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IReadOnlyList<SubcategoryDto>> Handle(
            GetSubcategoriesByCategoryQuery request,
            CancellationToken cancellationToken)
        {
            var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);

            if (!categoryExists)
            {
                throw new NotFoundException(nameof(Category), request.CategoryId);
            }

            var subcategories = await _subcategoryRepository.GetByCategoryIdAsync(request.CategoryId, cancellationToken);

            return subcategories.Select(s => new SubcategoryDto(s.Id, s.Name, s.CategoryId)).ToList();
        }
    }
}
