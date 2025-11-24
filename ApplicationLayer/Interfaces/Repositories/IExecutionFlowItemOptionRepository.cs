using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IExecutionFlowItemOptionRepository
{
    Task<ExecutionFlowItemOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowItemOption>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowItemOption>> GetByExecutionFlowStepItemIdAsync(Guid executionFlowStepItemId, CancellationToken cancellationToken = default);
    Task<ExecutionFlowItemOption> CreateAsync(ExecutionFlowItemOption executionFlowItemOption, CancellationToken cancellationToken = default);
    Task<ExecutionFlowItemOption> UpdateAsync(ExecutionFlowItemOption executionFlowItemOption, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

