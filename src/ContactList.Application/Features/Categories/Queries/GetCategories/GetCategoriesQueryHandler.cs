using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Categories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Categories.Queries.GetCategories
{
    /// <summary>
    /// Loads every category and projects it to <see cref="CategoryDto"/>. The mapping
    /// is done inline rather than through AutoMapper — two fields don't justify the
    /// overhead of a profile.
    /// </summary>
    public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList();
        }
    }
}
