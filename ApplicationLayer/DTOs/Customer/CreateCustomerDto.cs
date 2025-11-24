using DomainLayer.ValueObjects;

namespace ApplicationLayer.DTOs.Customer;

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}
