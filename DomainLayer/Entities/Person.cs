using DomainLayer.ValueObjects;

namespace DomainLayer.Entities;

public class Person : Entity
{
    public Name Name { get; private set; } = null!;
    public string Document { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public Email Email { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;

    protected Person()
    {
        // For ORM
        Document = string.Empty;
    }

    protected Person(
        Name name,
        string document,
        DateTime birthDate,
        Email email,
        Address address,
        PhoneNumber phoneNumber)
        : base()
    {
        Name = name;
        Document = document ?? throw new ArgumentNullException(nameof(document));
        BirthDate = birthDate;
        Email = email;
        Address = address;
        PhoneNumber = phoneNumber;

        ValidateBirthDate();
    }

    public void UpdateName(Name name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        MarkAsUpdated();
    }

    public void UpdateEmail(Email email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        MarkAsUpdated();
    }

    public void UpdateAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        MarkAsUpdated();
    }

    public void UpdatePhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        MarkAsUpdated();
    }

    private void ValidateBirthDate()
    {
        if (BirthDate > DateTime.UtcNow)
            throw new ArgumentException("Birth date cannot be in the future", nameof(BirthDate));

        var age = DateTime.UtcNow.Year - BirthDate.Year;
        if (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear)
            age--;

        if (age < 0 || age > 150)
            throw new ArgumentException("Invalid birth date", nameof(BirthDate));
    }
}
