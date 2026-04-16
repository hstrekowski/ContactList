using FluentValidation;

namespace ContactList.Application.Features.Auth.Commands.Login
{
    /// <summary>
    /// Minimal validation for login — only checks that both fields are present and
    /// that the email is well-formed. Complexity rules are intentionally NOT applied
    /// here: a wrong password should always surface as a generic "invalid credentials"
    /// error from the handler, never as a 400 telling the attacker that the value
    /// failed a specific complexity rule.
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
