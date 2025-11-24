using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IExecutionFlowRepository
{
    Task<ExecutionFlow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlow>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ExecutionFlow> CreateAsync(ExecutionFlow executionFlow, CancellationToken cancellationToken = default);
    Task<ExecutionFlow> UpdateAsync(ExecutionFlow executionFlow, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

