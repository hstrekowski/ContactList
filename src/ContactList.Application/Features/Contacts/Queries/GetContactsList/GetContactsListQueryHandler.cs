using AutoMapper;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactsList
{
    /// <summary>
    /// Fetches all contacts and maps them to the list DTO. The repository should use eager loading for categories to avoid extra database hits during mapping.
    /// </summary>
    public sealed class GetContactsListQueryHandler : IRequestHandler<GetContactsListQuery, IReadOnlyList<ContactListItemDto>>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public GetContactsListQueryHandler(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ContactListItemDto>> Handle(GetContactsListQuery request, CancellationToken cancellationToken)
        {
            var contacts = await _contactRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<ContactListItemDto>>(contacts);
        }
    }
}
