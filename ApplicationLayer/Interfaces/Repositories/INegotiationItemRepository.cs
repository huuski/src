using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface INegotiationItemRepository
{
    Task<NegotiationItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationItem>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default);
    Task<NegotiationItem> CreateAsync(NegotiationItem negotiationItem, CancellationToken cancellationToken = default);
    Task<NegotiationItem> UpdateAsync(NegotiationItem negotiationItem, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

