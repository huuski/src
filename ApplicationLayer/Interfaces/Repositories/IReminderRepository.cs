using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IReminderRepository
{
    Task<Reminder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reminder>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Reminder> CreateAsync(Reminder reminder, CancellationToken cancellationToken = default);
    Task<Reminder> UpdateAsync(Reminder reminder, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

