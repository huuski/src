using ApplicationLayer.DTOs.ExecutionFlow;

namespace ApplicationLayer.Interfaces.Services;

public interface IExecutionFlowService
{
    Task<ExecutionFlowDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExecutionFlowCompleteDto> GetByIdCompleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionFlowDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ExecutionFlowDto> CreateAsync(CreateExecutionFlowDto dto, CancellationToken cancellationToken = default);
    Task<ExecutionFlowDto> UpdateAsync(Guid id, UpdateExecutionFlowDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

