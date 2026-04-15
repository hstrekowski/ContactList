using ContactList.Domain.Common;
using ContactList.Domain.Exceptions;
using ContactList.Domain.ValueObjects;

namespace ContactList.Domain.Entities
{
    /// <summary>
    /// Contact entry in the contact list.
    /// </summary>
    public class Contact : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = string.Empty;
        public PhoneNumber PhoneNumber { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }

        public Category Category { get; private set; } = null!;
        public Guid CategoryId { get; private set; }

        public Subcategory? Subcategory { get; private set; }
        public Guid? SubcategoryId { get; private set; }

        protected Contact() : base() { }

        public Contact(
            string firstName,
            string lastName,
            Email email,
            string passwordHash,
            PhoneNumber phoneNumber,
            DateOnly dateOfBirth,
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
            Email email,
            PhoneNumber phoneNumber,
            DateOnly dateOfBirth,
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

        private void SetEmail(Email value)
        {
            Email = value ?? throw new DomainException("Email is required.");
        }

        private void SetPasswordHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Password hash is required.");

            PasswordHash = value;
        }

        private void SetPhoneNumber(PhoneNumber value)
        {
            PhoneNumber = value ?? throw new DomainException("Phone number is required.");
        }

        private void SetDateOfBirth(DateOnly value)
        {
            if (value > DateOnly.FromDateTime(DateTime.UtcNow))
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
