using ContactList.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ContactList.Domain.ValueObjects
{
    /// <summary>
    /// Represents a phone number in E.164 format (e.g. +48123456789).
    /// </summary>
    public sealed record PhoneNumber
    {
        private const int MinDigits = 7;
        private const int MaxDigits = 15;

        public string Value { get; }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Phone number cannot be empty.");

            var normalized = Regex.Replace(value, @"[\s-]", "");

            if (!Regex.IsMatch(normalized, @"^\+[0-9]+$"))
                throw new DomainException($"'{value}' is not a valid phone number. Expected international format, e.g. +48123456789.");

            var digitCount = normalized.Length - 1;
            if (digitCount < MinDigits || digitCount > MaxDigits)
                throw new DomainException($"Phone number must contain between {MinDigits} and {MaxDigits} digits.");

            Value = normalized;
        }

        public override string ToString() => Value;
    }
}
