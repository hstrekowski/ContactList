using ContactList.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ContactList.Domain.ValueObjects
{
    /// <summary>
    /// Represents a validated raw password that satisfies complexity requirements.
    /// </summary>
    public sealed record Password
    {
        private const int MinLength = 8;
        private const int MaxLength = 128;

        public string Value { get; }

        public Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Password cannot be empty.");

            if (value.Length < MinLength)
                throw new DomainException($"Password must be at least {MinLength} characters long.");

            if (value.Length > MaxLength)
                throw new DomainException($"Password cannot be longer than {MaxLength} characters.");

            if (!Regex.IsMatch(value, @"[A-Z]"))
                throw new DomainException("Password must contain at least one uppercase letter.");

            if (!Regex.IsMatch(value, @"[a-z]"))
                throw new DomainException("Password must contain at least one lowercase letter.");

            if (!Regex.IsMatch(value, @"[0-9]"))
                throw new DomainException("Password must contain at least one digit.");

            if (!Regex.IsMatch(value, @"[^a-zA-Z0-9]"))
                throw new DomainException("Password must contain at least one special character.");

            Value = value;
        }

        public override string ToString() => "********";
    }
}
