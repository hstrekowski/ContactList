using ContactList.Domain.Common;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Entities
{
    /// <summary>
    /// Subcategory entity scoped to a single Category
    /// </summary>
    public class Subcategory : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;

        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;

        protected Subcategory() : base() { }
        public Subcategory(string name, Guid categoryId) : base()
        {
            if (categoryId == Guid.Empty)
                throw new DomainException("Category ID cannot be empty.");

            SetName(name);
            CategoryId = categoryId;
        }

        public void Rename(string newName) => SetName(newName);

        private void SetName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new DomainException("Subcategory name cannot be empty.");

            Name = newName.Trim();
        }
    }
}
