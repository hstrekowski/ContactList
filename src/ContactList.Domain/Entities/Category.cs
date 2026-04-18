using ContactList.Domain.Common;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Entities
{
    /// <summary>
    ///  Category entity. holds the name and manages its subcategories.
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

            Name = name.Trim();
        }
    }
}
