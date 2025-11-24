using ApplicationLayer.DTOs.ExecutionFlowItemOption;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class ExecutionFlowItemOptionService : IExecutionFlowItemOptionService
{
    private readonly IExecutionFlowItemOptionRepository _executionFlowItemOptionRepository;

    public ExecutionFlowItemOptionService(IExecutionFlowItemOptionRepository executionFlowItemOptionRepository)
    {
        _executionFlowItemOptionRepository = executionFlowItemOptionRepository ?? throw new ArgumentNullException(nameof(executionFlowItemOptionRepository));
    }

    public async Task<ExecutionFlowItemOptionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlowItemOption = await _executionFlowItemOptionRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowItemOption == null)
            throw new ArgumentException($"ExecutionFlowItemOption with id {id} not found", nameof(id));

        return MapToDto(executionFlowItemOption);
    }

    public async Task<IEnumerable<ExecutionFlowItemOptionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var executionFlowItemOptions = await _executionFlowItemOptionRepository.GetAllAsync(cancellationToken);
        return executionFlowItemOptions.Select(MapToDto);
    }

    public async Task<IEnumerable<ExecutionFlowItemOptionDto>> GetByExecutionFlowStepItemIdAsync(Guid executionFlowStepItemId, CancellationToken cancellationToken = default)
    {
        var executionFlowItemOptions = await _executionFlowItemOptionRepository.GetByExecutionFlowStepItemIdAsync(executionFlowStepItemId, cancellationToken);
        return executionFlowItemOptions.Select(MapToDto);
    }

    public async Task<ExecutionFlowItemOptionDto> CreateAsync(CreateExecutionFlowItemOptionDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var executionFlowItemOption = new ExecutionFlowItemOption(
            dto.ExecutionFlowStepItemId,
            dto.Title,
            dto.Value,
            dto.Order,
            dto.EnableExtraAnswer,
            dto.ExtraAnswerMaxLength);

        var created = await _executionFlowItemOptionRepository.CreateAsync(executionFlowItemOption, cancellationToken);
        return MapToDto(created);
    }

    public async Task<ExecutionFlowItemOptionDto> UpdateAsync(Guid id, UpdateExecutionFlowItemOptionDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var executionFlowItemOption = await _executionFlowItemOptionRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowItemOption == null)
            throw new ArgumentException($"ExecutionFlowItemOption with id {id} not found", nameof(id));

        executionFlowItemOption.Update(dto.Title, dto.Value, dto.Order, dto.EnableExtraAnswer, dto.ExtraAnswerMaxLength);

        var updated = await _executionFlowItemOptionRepository.UpdateAsync(executionFlowItemOption, cancellationToken);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _executionFlowItemOptionRepository.DeleteAsync(id, cancellationToken);
    }

    private static ExecutionFlowItemOptionDto MapToDto(ExecutionFlowItemOption executionFlowItemOption)
    {
        return new ExecutionFlowItemOptionDto
        {
            Id = executionFlowItemOption.Id,
            ExecutionFlowStepItemId = executionFlowItemOption.ExecutionFlowStepItemId,
            Title = executionFlowItemOption.Title,
            Value = executionFlowItemOption.Value,
            Order = executionFlowItemOption.Order,
            EnableExtraAnswer = executionFlowItemOption.EnableExtraAnswer,
            ExtraAnswerMaxLength = executionFlowItemOption.ExtraAnswerMaxLength,
            CreatedAt = executionFlowItemOption.CreatedAt,
            UpdatedAt = executionFlowItemOption.UpdatedAt
        };
    }
}

