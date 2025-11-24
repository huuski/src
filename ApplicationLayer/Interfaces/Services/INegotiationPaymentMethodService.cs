using ApplicationLayer.DTOs.NegotiationPaymentMethod;

namespace ApplicationLayer.Interfaces.Services;

public interface INegotiationPaymentMethodService
{
    Task<NegotiationPaymentMethodDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationPaymentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationPaymentMethodDto>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default);
    Task<NegotiationPaymentMethodDto> CreateAsync(CreateNegotiationPaymentMethodDto dto, CancellationToken cancellationToken = default);
    Task<NegotiationPaymentMethodDto> UpdateAsync(Guid id, UpdateNegotiationPaymentMethodDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

