namespace DomainLayer.ValueObjects;

public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string? Country { get; private set; }
    public string? Complement { get; private set; }

    private Address(string street, string city, string state, string zipCode, string? country = null, string? complement = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));

        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be null or empty", nameof(state));

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode cannot be null or empty", nameof(zipCode));

        Street = street.Trim();
        City = city.Trim();
        State = state.Trim();
        ZipCode = zipCode.Trim();
        Country = country?.Trim();
        Complement = complement?.Trim();
    }

    public static Address Create(string street, string city, string state, string zipCode, string? country = null, string? complement = null)
    {
        return new Address(street, city, state, zipCode, country, complement);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Address other)
            return Street == other.Street &&
                   City == other.City &&
                   State == other.State &&
                   ZipCode == other.ZipCode &&
                   Country == other.Country &&
                   Complement == other.Complement;

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, ZipCode, Country, Complement);
    }

    public override string ToString()
    {
        var parts = new List<string> { Street };
        
        if (!string.IsNullOrWhiteSpace(Complement))
            parts.Add(Complement);
            
        parts.Add($"{City}, {State} {ZipCode}");
        
        if (!string.IsNullOrWhiteSpace(Country))
            parts.Add(Country);
            
        return string.Join(", ", parts);
    }
}
