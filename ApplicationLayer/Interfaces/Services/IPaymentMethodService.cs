using ApplicationLayer.DTOs.PaymentMethod;

namespace ApplicationLayer.Interfaces.Services;

public interface IPaymentMethodService
{
    Task<PaymentMethodDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto, CancellationToken cancellationToken = default);
    Task<PaymentMethodDto> UpdateAsync(Guid id, UpdatePaymentMethodDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentMethodDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentMethodDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}

