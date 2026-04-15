using ContactList.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ContactList.Domain.ValueObjects
{
    /// <summary>
    /// Represents a validated and normalized email address.
    /// </summary>
    public record Email
    {
        private const int MaxLength = 254;

        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Email cannot be empty.");

            value = value.Trim();

            if (value.Length > MaxLength)
                throw new DomainException($"Email cannot be longer than {MaxLength} characters.");

            if (!EmailRegex.IsMatch(value))
                throw new DomainException("Email format is invalid.");

            Value = value.ToLowerInvariant();
        }
        public override string ToString() => Value;
    }
}