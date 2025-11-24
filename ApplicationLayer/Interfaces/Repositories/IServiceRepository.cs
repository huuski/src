using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Service> CreateAsync(Service service, CancellationToken cancellationToken = default);
    Task<Service> UpdateAsync(Service service, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

