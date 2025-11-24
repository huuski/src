namespace DomainLayer.ValueObjects;

public class Name
{
    public string Value { get; private set; }

    private Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name cannot be null or empty", nameof(value));

        if (value.Length < 2)
            throw new ArgumentException("Name must be at least 2 characters long", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Name must not exceed 100 characters", nameof(value));

        Value = value.Trim();
    }

    public static Name Create(string value)
    {
        return new Name(value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Name other)
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

    public static implicit operator string(Name name)
    {
        return name.Value;
    }
}
