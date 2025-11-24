using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface ISupplyRepository
{
    Task<Supply?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Supply>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Supply> CreateAsync(Supply supply, CancellationToken cancellationToken = default);
    Task<Supply> UpdateAsync(Supply supply, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

