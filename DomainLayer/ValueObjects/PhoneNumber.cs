using System.Text.RegularExpressions;

namespace DomainLayer.ValueObjects;

public class PhoneNumber
{
    private static readonly Regex PhoneRegex = new(
        @"^[\d\s\-\+\(\)]+$",
        RegexOptions.Compiled);

    public string Value { get; private set; }

    private PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(value));

        var digitsOnly = Regex.Replace(value, @"\D", "");
        
        if (digitsOnly.Length < 10 || digitsOnly.Length > 15)
            throw new ArgumentException("Phone number must contain between 10 and 15 digits", nameof(value));

        if (!PhoneRegex.IsMatch(value))
            throw new ArgumentException("Invalid phone number format", nameof(value));

        Value = value.Trim();
    }

    public static PhoneNumber Create(string value)
    {
        return new PhoneNumber(value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is PhoneNumber other)
            return Value == other.Value;

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(PhoneNumber phoneNumber)
    {
        return phoneNumber.Value;
    }
}
