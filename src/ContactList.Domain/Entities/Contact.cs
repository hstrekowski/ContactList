using ContactList.Domain.Common;
using ContactList.Domain.Exceptions;

namespace ContactList.Domain.Entities
{
    /// <summary>
    /// Contact entry in the contact list.
    /// </summary>
    public class Contact : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public DateTime DateOfBirth { get; private set; }

        public Category Category { get; private set; } = null!;
        public Guid CategoryId { get; private set; }

        public Subcategory? Subcategory { get; private set; }
        public Guid? SubcategoryId { get; private set; }

        protected Contact() : base() { }

        public Contact(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            string phoneNumber,
            DateTime dateOfBirth,
            Guid categoryId,
            Guid? subcategoryId) : base()
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            SetEmail(email);
            SetPasswordHash(passwordHash);
            SetPhoneNumber(phoneNumber);
            SetDateOfBirth(dateOfBirth);
            SetCategory(categoryId, subcategoryId);
        }

        public void Update(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            DateTime dateOfBirth,
            Guid categoryId,
            Guid? subcategoryId)
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
            SetDateOfBirth(dateOfBirth);
            SetCategory(categoryId, subcategoryId);
        }

        public void ChangePasswordHash(string newPasswordHash) => SetPasswordHash(newPasswordHash);

        private void SetFirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("First name cannot be empty.");

            FirstName = value.Trim();
        }

        private void SetLastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Last name cannot be empty.");

            LastName = value.Trim();
        }

        private void SetEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Email cannot be empty.");

            Email = value.Trim();
        }

        private void SetPasswordHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Password hash is required.");

            PasswordHash = value;
        }

        private void SetPhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Phone number cannot be empty.");

            PhoneNumber = value.Trim();
        }

        private void SetDateOfBirth(DateTime value)
        {
            if (value > DateTime.UtcNow)
                throw new DomainException("Date of birth cannot be in the future.");

            DateOfBirth = value;
        }

        private void SetCategory(Guid categoryId, Guid? subcategoryId)
        {
            if (categoryId == Guid.Empty)
                throw new DomainException("Contact must be assigned to a valid category.");

            if (subcategoryId == Guid.Empty)
                throw new DomainException("Subcategory id must be a valid Guid.");

            CategoryId = categoryId;
            SubcategoryId = subcategoryId;
        }
    }
}
