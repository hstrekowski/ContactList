using AutoMapper;
using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Domain.Entities;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactById
{
    /// <summary>
    /// Loads a single <see cref="Contact"/> by id and projects it to <see cref="ContactDetailDto"/>.
    /// Throws <see cref="NotFoundException"/> when no contact matches, so the API layer can
    /// translate it into a 404 response via the global exception handler.
    /// </summary>
    public sealed class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactDetailDto>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public GetContactByIdQueryHandler(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<ContactDetailDto> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Contact), request.Id);

            return _mapper.Map<ContactDetailDto>(contact);
        }
    }
}
