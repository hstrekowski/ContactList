using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Categories.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Categories.Queries.GetCategories
{
    /// <summary>
    /// Loads all categories and maps them to DTOs. Mapping is done manually since it is only two fields.
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
