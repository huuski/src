using ApplicationLayer.DTOs.ExecutionFlowStep;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class ExecutionFlowStepService : IExecutionFlowStepService
{
    private readonly IExecutionFlowStepRepository _executionFlowStepRepository;

    public ExecutionFlowStepService(IExecutionFlowStepRepository executionFlowStepRepository)
    {
        _executionFlowStepRepository = executionFlowStepRepository ?? throw new ArgumentNullException(nameof(executionFlowStepRepository));
    }

    public async Task<ExecutionFlowStepDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlowStep = await _executionFlowStepRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowStep == null)
            throw new ArgumentException($"ExecutionFlowStep with id {id} not found", nameof(id));

        return MapToDto(executionFlowStep);
    }

    public async Task<IEnumerable<ExecutionFlowStepDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var executionFlowSteps = await _executionFlowStepRepository.GetAllAsync(cancellationToken);
        return executionFlowSteps.Select(MapToDto);
    }

    public async Task<IEnumerable<ExecutionFlowStepDto>> GetByExecutionFlowIdAsync(Guid executionFlowId, CancellationToken cancellationToken = default)
    {
        var executionFlowSteps = await _executionFlowStepRepository.GetByExecutionFlowIdAsync(executionFlowId, cancellationToken);
        return executionFlowSteps.Select(MapToDto);
    }

    public async Task<ExecutionFlowStepDto> CreateAsync(CreateExecutionFlowStepDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var executionFlowStep = new ExecutionFlowStep(
            dto.ExecutionFlowId,
            dto.StepNumber,
            dto.Title,
            dto.Description,
            dto.DisplayStepNumber,
            dto.DisplayTitle,
            dto.Subtitle
        );

        var createdExecutionFlowStep = await _executionFlowStepRepository.CreateAsync(executionFlowStep, cancellationToken);
        return MapToDto(createdExecutionFlowStep);
    }

    public async Task<ExecutionFlowStepDto> UpdateAsync(Guid id, UpdateExecutionFlowStepDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var executionFlowStep = await _executionFlowStepRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowStep == null)
            throw new ArgumentException($"ExecutionFlowStep with id {id} not found", nameof(id));

        executionFlowStep.Update(
            dto.StepNumber,
            dto.Title,
            dto.Description,
            dto.DisplayStepNumber,
            dto.DisplayTitle,
            dto.Subtitle
        );

        var updatedExecutionFlowStep = await _executionFlowStepRepository.UpdateAsync(executionFlowStep, cancellationToken);
        return MapToDto(updatedExecutionFlowStep);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlowStep = await _executionFlowStepRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowStep == null)
            throw new ArgumentException($"ExecutionFlowStep with id {id} not found", nameof(id));

        return await _executionFlowStepRepository.DeleteAsync(id, cancellationToken);
    }

    private static ExecutionFlowStepDto MapToDto(ExecutionFlowStep executionFlowStep)
    {
        return new ExecutionFlowStepDto
        {
            Id = executionFlowStep.Id,
            ExecutionFlowId = executionFlowStep.ExecutionFlowId,
            StepNumber = executionFlowStep.StepNumber,
            Title = executionFlowStep.Title,
            Subtitle = executionFlowStep.Subtitle,
            Description = executionFlowStep.Description,
            DisplayStepNumber = executionFlowStep.DisplayStepNumber,
            DisplayTitle = executionFlowStep.DisplayTitle,
            CreatedAt = executionFlowStep.CreatedAt,
            UpdatedAt = executionFlowStep.UpdatedAt
        };
    }
}

