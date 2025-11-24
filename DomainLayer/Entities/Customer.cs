using DomainLayer.ValueObjects;

namespace DomainLayer.Entities;

public class Customer : Person
{
    private Customer()
    {
        // For ORM
    }

    public Customer(
        Name name,
        string document,
        DateTime birthDate,
        Email email,
        Address address,
        PhoneNumber phoneNumber)
        : base(name, document, birthDate, email, address, phoneNumber)
    {
    }
}
