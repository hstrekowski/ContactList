using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Subcategories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Subcategories.Queries.GetSubcategoriesByCategory
{
    /// <summary>
    /// Loads every subcategory bound to the given category and projects each row to
    /// <see cref="SubcategoryDto"/>. An unknown category id and a category with no
    /// subcategories (e.g. "Prywatny") both return an empty list — the distinction
    /// is irrelevant to the dropdown that consumes this endpoint.
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
