using AutoMapper;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Features.Contacts.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactsList
{
    /// <summary>
    /// Loads every contact and projects each row to <see cref="ContactListItemDto"/>.
    /// The repository is expected to eagerly load the <c>Category</c> navigation so the
    /// AutoMapper profile can read <c>CategoryName</c> without lazy-load round trips.
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
