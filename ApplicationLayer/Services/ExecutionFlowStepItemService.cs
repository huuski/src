using ApplicationLayer.DTOs.ExecutionFlowItemOption;
using ApplicationLayer.DTOs.ExecutionFlowStepItem;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class ExecutionFlowStepItemService : IExecutionFlowStepItemService
{
    private readonly IExecutionFlowStepItemRepository _executionFlowStepItemRepository;
    private readonly IExecutionFlowItemOptionRepository _executionFlowItemOptionRepository;

    public ExecutionFlowStepItemService(
        IExecutionFlowStepItemRepository executionFlowStepItemRepository,
        IExecutionFlowItemOptionRepository executionFlowItemOptionRepository)
    {
        _executionFlowStepItemRepository = executionFlowStepItemRepository ?? throw new ArgumentNullException(nameof(executionFlowStepItemRepository));
        _executionFlowItemOptionRepository = executionFlowItemOptionRepository ?? throw new ArgumentNullException(nameof(executionFlowItemOptionRepository));
    }

    public async Task<ExecutionFlowStepItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlowStepItem = await _executionFlowStepItemRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowStepItem == null)
            throw new ArgumentException($"ExecutionFlowStepItem with id {id} not found", nameof(id));

        var options = await _executionFlowItemOptionRepository.GetByExecutionFlowStepItemIdAsync(id, cancellationToken);
        return MapToDto(executionFlowStepItem, options);
    }

    public async Task<IEnumerable<ExecutionFlowStepItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var executionFlowStepItems = await _executionFlowStepItemRepository.GetAllAsync(cancellationToken);
        var result = new List<ExecutionFlowStepItemDto>();
        
        foreach (var item in executionFlowStepItems)
        {
            var options = await _executionFlowItemOptionRepository.GetByExecutionFlowStepItemIdAsync(item.Id, cancellationToken);
            result.Add(MapToDto(item, options));
        }
        
        return result;
    }

    public async Task<IEnumerable<ExecutionFlowStepItemDto>> GetByExecutionFlowStepIdAsync(Guid executionFlowStepId, CancellationToken cancellationToken = default)
    {
        var executionFlowStepItems = await _executionFlowStepItemRepository.GetByExecutionFlowStepIdAsync(executionFlowStepId, cancellationToken);
        var result = new List<ExecutionFlowStepItemDto>();
        
        foreach (var item in executionFlowStepItems)
        {
            var options = await _executionFlowItemOptionRepository.GetByExecutionFlowStepItemIdAsync(item.Id, cancellationToken);
            result.Add(MapToDto(item, options));
        }
        
        return result;
    }

    public async Task<ExecutionFlowStepItemDto> CreateAsync(CreateExecutionFlowStepItemDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Validar que Options é obrigatório quando Type é Radio ou Multiselect
        if ((dto.Type == DomainLayer.Enums.AnswerType.Radio || dto.Type == DomainLayer.Enums.AnswerType.Multiselect) &&
            (dto.Options == null || dto.Options.Count == 0))
        {
            throw new ArgumentException("Options is required when Type is Radio or Multiselect", nameof(dto));
        }

        var executionFlowStepItem = new ExecutionFlowStepItem(
            dto.ExecutionFlowStepId,
            dto.Order,
            dto.Title,
            dto.Type,
            dto.Required,
            dto.Subtitle,
            dto.Description,
            dto.MaxLength,
            dto.MaxImages,
            dto.AcceptedImageTypes
        );

        var createdExecutionFlowStepItem = await _executionFlowStepItemRepository.CreateAsync(executionFlowStepItem, cancellationToken);

        // Criar as opções se fornecidas
        if (dto.Options != null && dto.Options.Count > 0)
        {
            foreach (var optionDto in dto.Options)
            {
                var option = new DomainLayer.Entities.ExecutionFlowItemOption(
                    createdExecutionFlowStepItem.Id,
                    optionDto.Title,
                    optionDto.Value,
                    optionDto.Order,
                    optionDto.EnableExtraAnswer,
                    optionDto.ExtraAnswerMaxLength
                );
                await _executionFlowItemOptionRepository.CreateAsync(option, cancellationToken);
            }
        }

        var options = await _executionFlowItemOptionRepository.GetByExecutionFlowStepItemIdAsync(createdExecutionFlowStepItem.Id, cancellationToken);
        return MapToDto(createdExecutionFlowStepItem, options);
    }

    public async Task<ExecutionFlowStepItemDto> UpdateAsync(Guid id, UpdateExecutionFlowStepItemDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Validar que Options é obrigatório quando Type é Radio ou Multiselect
        if ((dto.Type == DomainLayer.Enums.AnswerType.Radio || dto.Type == DomainLayer.Enums.AnswerType.Multiselect) &&
            (dto.Options == null || dto.Options.Count == 0))
        {
            throw new ArgumentException("Options is required when Type is Radio or Multiselect", nameof(dto));
        }

        var executionFlowStepItem = await _executionFlowStepItemRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowStepItem == null)
            throw new ArgumentException($"ExecutionFlowStepItem with id {id} not found", nameof(id));

        executionFlowStepItem.Update(
            dto.Order,
            dto.Title,
            dto.Type,
            dto.Required,
            dto.Subtitle,
            dto.Description,
            dto.MaxLength,
            dto.MaxImages,
            dto.AcceptedImageTypes
        );

        var updatedExecutionFlowStepItem = await _executionFlowStepItemRepository.UpdateAsync(executionFlowStepItem, cancellationToken);

        // Remover opções existentes e criar novas se fornecidas
        var existingOptions = await _executionFlowItemOptionRepository.GetByExecutionFlowStepItemIdAsync(id, cancellationToken);
        foreach (var existingOption in existingOptions)
        {
            await _executionFlowItemOptionRepository.DeleteAsync(existingOption.Id, cancellationToken);
        }

        // Criar as novas opções se fornecidas
        if (dto.Options != null && dto.Options.Count > 0)
        {
            foreach (var optionDto in dto.Options)
            {
                var option = new DomainLayer.Entities.ExecutionFlowItemOption(
                    updatedExecutionFlowStepItem.Id,
                    optionDto.Title,
                    optionDto.Value,
                    optionDto.Order,
                    optionDto.EnableExtraAnswer,
                    optionDto.ExtraAnswerMaxLength
                );
                await _executionFlowItemOptionRepository.CreateAsync(option, cancellationToken);
            }
        }

        var options = await _executionFlowItemOptionRepository.GetByExecutionFlowStepItemIdAsync(updatedExecutionFlowStepItem.Id, cancellationToken);
        return MapToDto(updatedExecutionFlowStepItem, options);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var executionFlowStepItem = await _executionFlowStepItemRepository.GetByIdAsync(id, cancellationToken);
        if (executionFlowStepItem == null)
            throw new ArgumentException($"ExecutionFlowStepItem with id {id} not found", nameof(id));

        return await _executionFlowStepItemRepository.DeleteAsync(id, cancellationToken);
    }

    private static ExecutionFlowStepItemDto MapToDto(ExecutionFlowStepItem executionFlowStepItem, IEnumerable<DomainLayer.Entities.ExecutionFlowItemOption> options)
    {
        return new ExecutionFlowStepItemDto
        {
            Id = executionFlowStepItem.Id,
            ExecutionFlowStepId = executionFlowStepItem.ExecutionFlowStepId,
            Order = executionFlowStepItem.Order,
            Title = executionFlowStepItem.Title,
            Subtitle = executionFlowStepItem.Subtitle,
            Description = executionFlowStepItem.Description,
            Type = executionFlowStepItem.AnswerType,
            MaxLength = executionFlowStepItem.MaxLength,
            Required = executionFlowStepItem.Required,
            MaxImages = executionFlowStepItem.MaxImages,
            AcceptedImageTypes = executionFlowStepItem.AcceptedImageTypes,
            Options = options.Select(o => new ExecutionFlowItemOptionDto
            {
                Id = o.Id,
                ExecutionFlowStepItemId = o.ExecutionFlowStepItemId,
                Title = o.Title,
                Value = o.Value,
                Order = o.Order,
                EnableExtraAnswer = o.EnableExtraAnswer,
                ExtraAnswerMaxLength = o.ExtraAnswerMaxLength,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            }).OrderBy(o => o.Order).ToList(),
            CreatedAt = executionFlowStepItem.CreatedAt,
            UpdatedAt = executionFlowStepItem.UpdatedAt
        };
    }
}

