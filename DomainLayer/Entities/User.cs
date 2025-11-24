using DomainLayer.ValueObjects;

namespace DomainLayer.Entities;

public class User : Person
{
    public Password Password { get; private set; } = null!;
    public string? Avatar { get; private set; }

    private User() { }

    public User(
        Name name,
        string document,
        DateTime birthDate,
        Email email,
        Address address,
        PhoneNumber phoneNumber,
        Password password,
        string? avatar = null)
        : base(name, document, birthDate, email, address, phoneNumber)
    {
        Password = password ?? throw new ArgumentNullException(nameof(password));
        Avatar = avatar;
    }

    public void UpdatePassword(Password newPassword)
    {
        Password = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
        MarkAsUpdated();
    }

    public void UpdateAvatar(string? avatar)
    {
        Avatar = avatar;
        MarkAsUpdated();
    }
}
