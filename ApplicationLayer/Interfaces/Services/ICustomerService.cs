using ApplicationLayer.DTOs.Customer;

namespace ApplicationLayer.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
