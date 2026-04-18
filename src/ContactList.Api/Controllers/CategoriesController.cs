using ContactList.Application.Features.Categories.Queries.DTOs;
using ContactList.Application.Features.Categories.Queries.GetCategories;
using ContactList.Application.Features.Subcategories.Queries.DTOs;
using ContactList.Application.Features.Subcategories.Queries.GetSubcategoriesByCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Api.Controllers
{
    /// <summary>
    /// Dictionary endpoints for categories and subcategories. 
    /// Used to populate dropdowns in the contact form.
    /// </summary>
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public sealed class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns all available contact categories.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Returns subcategories for a specific category.
        /// </summary>
        /// <param name="categoryId">The unique ID of the parent category.</param>
        [HttpGet("{categoryId:guid}/subcategories")]
        [ProducesResponseType(typeof(IReadOnlyList<SubcategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<SubcategoryDto>>> GetSubcategories(
            [FromRoute] Guid categoryId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSubcategoriesByCategoryQuery(categoryId), cancellationToken);
            return Ok(result);
        }
    }
}