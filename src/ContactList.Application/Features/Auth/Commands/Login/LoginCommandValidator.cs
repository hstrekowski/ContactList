using FluentValidation;

namespace ContactList.Application.Features.Auth.Commands.Login
{
    /// <summary>
    /// Basic validation for login. Just checks if fields are filled and the email looks okay. We skip complexity checks on the password to avoid giving away hints to attackers.
    /// </summary>
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
