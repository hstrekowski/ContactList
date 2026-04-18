using ContactList.Application.Features.Contacts.Commands.CreateContact;
using ContactList.Application.Features.Contacts.Commands.DeleteContact;
using ContactList.Application.Features.Contacts.Commands.UpdateContact;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Application.Features.Contacts.Queries.GetContactById;
using ContactList.Application.Features.Contacts.Queries.GetContactsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Api.Controllers;

/// <summary>
/// Exposes CRUD operations for contacts. The two GET endpoints are public (per the
/// recruitment spec — unauthenticated users may browse the list and open details),
/// while create / update / delete require a valid JWT.
/// </summary>
[ApiController]
[Route("api/contacts")]
[Authorize]
public sealed class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Returns every contact as a slim list projection.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ContactListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContactListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetContactsListQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns the full details of a single contact.</summary>
    /// <response code="200">Contact found.</response>
    /// <response code="404">No contact with the given id.</response>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ContactDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetContactByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new contact.</summary>
    /// <response code="201">Contact created; <c>Location</c> header points to the details endpoint.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Missing or invalid JWT.</response>
    /// <response code="404">Referenced category or subcategory does not exist.</response>
    /// <response code="409">A contact with this email already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateContactCommand command,
        CancellationToken cancellationToken)
    {
        var newId = await _mediator.Send(command, cancellationToken);
        return CreatedAtRoute(nameof(GetById), new { id = newId }, newId);
    }

    /// <summary>Replaces an existing contact.</summary>
    /// <response code="204">Contact updated.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Missing or invalid JWT.</response>
    /// <response code="404">No contact with the given id, or referenced category / subcategory missing.</response>
    /// <response code="409">The email is already used by another contact.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateContactCommand command,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(command with { Id = id }, cancellationToken);
        return NoContent();
    }

    /// <summary>Deletes a contact.</summary>
    /// <response code="204">Contact deleted.</response>
    /// <response code="401">Missing or invalid JWT.</response>
    /// <response code="404">No contact with the given id.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteContactCommand(id), cancellationToken);
        return NoContent();
    }
}
