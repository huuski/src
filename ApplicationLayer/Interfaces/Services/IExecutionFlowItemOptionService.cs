using ApplicationLayer.DTOs.ExecutionFlowItemOption;

namespace ApplicationLayer.Interfaces.Services;

public interface IExecutionFlowItemOptionService
{
    Task<ExecutionFlowItemOptionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowItemOptionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowItemOptionDto>> GetByExecutionFlowStepItemIdAsync(Guid executionFlowStepItemId, CancellationToken cancellationToken = default);
    Task<ExecutionFlowItemOptionDto> CreateAsync(CreateExecutionFlowItemOptionDto dto, CancellationToken cancellationToken = default);
    Task<ExecutionFlowItemOptionDto> UpdateAsync(Guid id, UpdateExecutionFlowItemOptionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

