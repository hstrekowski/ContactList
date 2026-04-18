using AutoMapper;
using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using ContactList.Domain.Entities;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactById
{
    /// <summary>
    /// Loads a contact by ID and maps it to a detail DTO. Throws a NotFoundException if the ID doesn't exist, which the API converts to a 404.
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
