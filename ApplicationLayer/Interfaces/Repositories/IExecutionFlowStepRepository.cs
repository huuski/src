using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IExecutionFlowStepRepository
{
    Task<ExecutionFlowStep?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStep>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStep>> GetByExecutionFlowIdAsync(Guid executionFlowId, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStep> CreateAsync(ExecutionFlowStep executionFlowStep, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStep> UpdateAsync(ExecutionFlowStep executionFlowStep, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

