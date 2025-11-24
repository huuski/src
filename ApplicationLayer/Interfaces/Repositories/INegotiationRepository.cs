using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface INegotiationRepository
{
    Task<Negotiation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Negotiation?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Negotiation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Negotiation>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Negotiation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Negotiation> CreateAsync(Negotiation negotiation, CancellationToken cancellationToken = default);
    Task<Negotiation> UpdateAsync(Negotiation negotiation, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

