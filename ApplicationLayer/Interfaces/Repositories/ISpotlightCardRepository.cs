using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface ISpotlightCardRepository
{
    Task<SpotlightCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SpotlightCard>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SpotlightCard> CreateAsync(SpotlightCard spotlightCard, CancellationToken cancellationToken = default);
    Task<SpotlightCard> UpdateAsync(SpotlightCard spotlightCard, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

