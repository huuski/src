using ApplicationLayer.DTOs.Customer;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.ValueObjects;

namespace ApplicationLayer.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
            throw new ArgumentException($"Customer with id {id} not found", nameof(id));

        return MapToDto(customer);
    }

    public async Task<CustomerDto?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("Document cannot be empty", nameof(document));

        var customer = await _customerRepository.GetByDocumentAsync(document, cancellationToken);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        var customer = await _customerRepository.GetByEmailAsync(email, cancellationToken);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<CustomerDto?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        var customer = await _customerRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var name = Name.Create(dto.Name);
        var email = Email.Create(dto.Email);
        var address = Address.Create(
            dto.Street,
            dto.City,
            dto.State,
            dto.ZipCode,
            dto.Country,
            dto.Complement
        );
        var phoneNumber = PhoneNumber.Create(dto.PhoneNumber);

        var customer = new Customer(
            name,
            dto.Document,
            dto.BirthDate,
            email,
            address,
            phoneNumber
        );

        var createdCustomer = await _customerRepository.CreateAsync(customer, cancellationToken);
        return MapToDto(createdCustomer);
    }

    public async Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
            throw new ArgumentException($"Customer with id {id} not found", nameof(id));

        customer.UpdateName(Name.Create(dto.Name));
        customer.UpdateEmail(Email.Create(dto.Email));
        customer.UpdatePhoneNumber(PhoneNumber.Create(dto.PhoneNumber));
        customer.UpdateAddress(Address.Create(
            dto.Street,
            dto.City,
            dto.State,
            dto.ZipCode,
            dto.Country,
            dto.Complement
        ));

        var updatedCustomer = await _customerRepository.UpdateAsync(customer, cancellationToken);
        return MapToDto(updatedCustomer);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
            throw new ArgumentException($"Customer with id {id} not found", nameof(id));

        return await _customerRepository.DeleteAsync(id, cancellationToken);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name.Value,
            Document = customer.Document,
            BirthDate = customer.BirthDate,
            Email = customer.Email.Value,
            Street = customer.Address.Street,
            City = customer.Address.City,
            State = customer.Address.State,
            ZipCode = customer.Address.ZipCode,
            Country = customer.Address.Country ?? string.Empty,
            Complement = customer.Address.Complement,
            PhoneNumber = customer.PhoneNumber.Value,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
