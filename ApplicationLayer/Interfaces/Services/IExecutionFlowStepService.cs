using ApplicationLayer.DTOs.ExecutionFlowStep;

namespace ApplicationLayer.Interfaces.Services;

public interface IExecutionFlowStepService
{
    Task<ExecutionFlowStepDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStepDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStepDto>> GetByExecutionFlowIdAsync(Guid executionFlowId, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStepDto> CreateAsync(CreateExecutionFlowStepDto dto, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStepDto> UpdateAsync(Guid id, UpdateExecutionFlowStepDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

