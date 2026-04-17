using FluentValidation;

namespace ContactList.Application.Features.Contacts.Commands.CreateContact
{
    /// <summary>
    /// Field-level validation for <see cref="CreateContactCommand"/>. Mirrors the rules
    /// enforced by the domain value objects so the API can return a 400 ProblemDetails
    /// with all errors at once instead of bubbling up the first <c>DomainException</c>.
    /// Cross-aggregate rules (category-vs-subcategory consistency, email uniqueness)
    /// live in the handler — they require database lookups.
    /// </summary>
    public sealed class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
    {
        public CreateContactCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot be longer than 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot be longer than 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(254).WithMessage("Email cannot be longer than 254 characters.")
                .EmailAddress().WithMessage("Email format is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .MaximumLength(128).WithMessage("Password cannot be longer than 128 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+[0-9\s-]+$").WithMessage("Phone number must be in international format, e.g. +48123456789.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth must be in the past.")
                .GreaterThan(new DateOnly(1900, 1, 1))
                .WithMessage("Date of birth must be after 1900-01-01.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.SubcategoryName)
                .MaximumLength(100).WithMessage("Subcategory name cannot be longer than 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubcategoryName));
        }
    }
}
