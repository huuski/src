using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface INegotiationPaymentMethodRepository
{
    Task<NegotiationPaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationPaymentMethod>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationPaymentMethod>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default);
    Task<NegotiationPaymentMethod> CreateAsync(NegotiationPaymentMethod negotiationPaymentMethod, CancellationToken cancellationToken = default);
    Task<NegotiationPaymentMethod> UpdateAsync(NegotiationPaymentMethod negotiationPaymentMethod, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

