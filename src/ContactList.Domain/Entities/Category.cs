using ContactList.Domain.Common;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Entities
{
    /// <summary>
    /// Top-level contact category entity.
    /// </summary>
    public class Category : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;

        private readonly List<Subcategory> _subcategories = new();
        public IReadOnlyCollection<Subcategory> Subcategories => _subcategories.AsReadOnly();

        protected Category() : base(){ }
         
        public Category(string name) : base()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name cannot be empty.");

            Name = name;
        }
    }
}
