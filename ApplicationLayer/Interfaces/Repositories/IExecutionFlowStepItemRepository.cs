using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IExecutionFlowStepItemRepository
{
    Task<ExecutionFlowStepItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStepItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStepItem>> GetByExecutionFlowStepIdAsync(Guid executionFlowStepId, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStepItem> CreateAsync(ExecutionFlowStepItem executionFlowStepItem, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStepItem> UpdateAsync(ExecutionFlowStepItem executionFlowStepItem, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

