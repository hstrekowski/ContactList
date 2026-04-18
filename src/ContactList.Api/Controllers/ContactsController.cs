using ContactList.Application.Features.Contacts.Commands.CreateContact;
using ContactList.Application.Features.Contacts.Commands.DeleteContact;
using ContactList.Application.Features.Contacts.Commands.UpdateContact;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Application.Features.Contacts.Queries.GetContactById;
using ContactList.Application.Features.Contacts.Queries.GetContactsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Api.Controllers
{
    /// <summary>
    /// Exposes CRUD operations for contacts. Public for browsing, protected for mutations.
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

        /// <summary>
        /// Returns every contact as a slim list projection. Accessible to unauthenticated users.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<ContactListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ContactListItemDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetContactsListQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Returns the full details of a single contact. Accessible to unauthenticated users.
        /// </summary>
        [HttpGet("{id:guid}", Name = nameof(GetById))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ContactDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactDetailDto>> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetContactByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new contact and returns its ID. Requires authentication.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateContactCommand command,
            CancellationToken cancellationToken)
        {
            var newId = await _mediator.Send(command, cancellationToken);
            return CreatedAtRoute(nameof(GetById), new { id = newId }, newId);
        }

        /// <summary>
        /// Replaces an existing contact. Requires authentication.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateContactCommand command,
            CancellationToken cancellationToken)
        {
            // We force the ID from the route into the command to ensure consistency
            await _mediator.Send(command with { Id = id }, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a contact. Requires authentication.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteContactCommand(id), cancellationToken);
            return NoContent();
        }
    }
}