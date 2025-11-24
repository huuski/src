using ApplicationLayer.DTOs.ExecutionFlow;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using System.Text.Json;

namespace ApplicationLayer.Services;

public class ExecutionFlowService : IExecutionFlowService
{
    private readonly IExecutionFlowRepository _executionFlowRepository;

    public ExecutionFlowService(IExecutionFlowRepository executionFlowRepository)
    {
        _executionFlowRepository = executionFlowRepository ?? throw new ArgumentNullException(nameof(executionFlowRepository));
    }

    public async Task<ExecutionFlowDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlow = await _executionFlowRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlow == null)
            throw new ArgumentException($"ExecutionFlow with id {id} not found", nameof(id));

        return MapToDto(executionFlow);
    }

    public async Task<ExecutionFlowCompleteDto> GetByIdCompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlow = await _executionFlowRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlow == null)
            throw new ArgumentException($"ExecutionFlow with id {id} not found", nameof(id));

        return MapToCompleteDto(executionFlow);
    }

    public async Task<IEnumerable<ExecutionFlowDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var executionFlows = await _executionFlowRepository.GetAllAsync(cancellationToken);
        return executionFlows.Select(MapToDto);
    }

    public async Task<ExecutionFlowDto> CreateAsync(CreateExecutionFlowDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Validar que Flow é um JSON válido
        ValidateJson(dto.Flow);

        var executionFlow = new ExecutionFlow(
            dto.Title,
            dto.Flow
        );

        var createdExecutionFlow = await _executionFlowRepository.CreateAsync(executionFlow, cancellationToken);
        return MapToDto(createdExecutionFlow);
    }

    public async Task<ExecutionFlowDto> UpdateAsync(Guid id, UpdateExecutionFlowDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Validar que Flow é um JSON válido
        ValidateJson(dto.Flow);

        var executionFlow = await _executionFlowRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlow == null)
            throw new ArgumentException($"ExecutionFlow with id {id} not found", nameof(id));

        executionFlow.Update(
            dto.Title,
            dto.Flow
        );

        var updatedExecutionFlow = await _executionFlowRepository.UpdateAsync(executionFlow, cancellationToken);
        return MapToDto(updatedExecutionFlow);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlow = await _executionFlowRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlow == null)
            throw new ArgumentException($"ExecutionFlow with id {id} not found", nameof(id));

        return await _executionFlowRepository.DeleteAsync(id, cancellationToken);
    }

    private static void ValidateJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("Flow JSON cannot be empty", nameof(json));

        try
        {
            JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON format in Flow: {ex.Message}", nameof(json), ex);
        }
    }

    private static ExecutionFlowDto MapToDto(ExecutionFlow executionFlow)
    {
        return new ExecutionFlowDto
        {
            Id = executionFlow.Id,
            Title = executionFlow.Title,
            Flow = executionFlow.Flow,
            CreatedAt = executionFlow.CreatedAt,
            UpdatedAt = executionFlow.UpdatedAt
        };
    }

    private static ExecutionFlowCompleteDto MapToCompleteDto(ExecutionFlow executionFlow)
    {
        // Parse o JSON Flow e extrair steps
        using var flowJson = JsonDocument.Parse(executionFlow.Flow);
        var rootElement = flowJson.RootElement;
        
        // Extrair steps do JSON - criar uma cópia do JsonElement para evitar problemas de dispose
        JsonElement stepsProperty;
        if (rootElement.TryGetProperty("steps", out var steps))
        {
            // Criar uma cópia do JsonElement serializando e deserializando
            var stepsJson = JsonSerializer.Serialize(steps);
            stepsProperty = JsonDocument.Parse(stepsJson).RootElement;
        }
        else
        {
            stepsProperty = JsonDocument.Parse("[]").RootElement;
        }

        return new ExecutionFlowCompleteDto
        {
            Id = executionFlow.Id,
            Title = executionFlow.Title,
            CreatedAt = executionFlow.CreatedAt,
            UpdatedAt = executionFlow.UpdatedAt,
            Steps = stepsProperty
        };
    }
}
