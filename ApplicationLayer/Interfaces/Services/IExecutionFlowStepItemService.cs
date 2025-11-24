using ApplicationLayer.DTOs.ExecutionFlowStepItem;

namespace ApplicationLayer.Interfaces.Services;

public interface IExecutionFlowStepItemService
{
    Task<ExecutionFlowStepItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStepItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowStepItemDto>> GetByExecutionFlowStepIdAsync(Guid executionFlowStepId, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStepItemDto> CreateAsync(CreateExecutionFlowStepItemDto dto, CancellationToken cancellationToken = default);
    Task<ExecutionFlowStepItemDto> UpdateAsync(Guid id, UpdateExecutionFlowStepItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

