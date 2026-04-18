using ContactList.Application.Features.Categories.Queries.DTOs;
using ContactList.Application.Features.Categories.Queries.GetCategories;
using ContactList.Application.Features.Subcategories.Queries.DTOs;
using ContactList.Application.Features.Subcategories.Queries.GetSubcategoriesByCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Api.Controllers;

/// <summary>
/// Dictionary endpoints used to populate the category and subcategory dropdowns on
/// the contact form. Requires authentication because the only consumer — the
/// create / edit screens — lives behind the login gate. Subcategories are exposed
/// as a nested resource (<c>/api/categories/{id}/subcategories</c>) to reflect the
/// parent-child relationship in the URL.
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

    /// <summary>Returns every seeded category (Służbowy / Prywatny / Inny).</summary>
    /// <response code="200">Categories returned.</response>
    /// <response code="401">Missing or invalid JWT.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns every subcategory belonging to the given category. An unknown
    /// category id yields an empty list (the handler treats "no rows" and "no
    /// such parent" identically).
    /// </summary>
    /// <response code="200">Subcategories returned (possibly empty).</response>
    /// <response code="401">Missing or invalid JWT.</response>
    [HttpGet("{categoryId:guid}/subcategories")]
    [ProducesResponseType(typeof(IReadOnlyList<SubcategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<SubcategoryDto>>> GetSubcategories(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSubcategoriesByCategoryQuery(categoryId), cancellationToken);
        return Ok(result);
    }
}
