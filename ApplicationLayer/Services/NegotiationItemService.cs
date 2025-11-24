using ApplicationLayer.DTOs.NegotiationItem;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;

namespace ApplicationLayer.Services;

public class NegotiationItemService : INegotiationItemService
{
    private readonly INegotiationItemRepository _negotiationItemRepository;
    private readonly INegotiationRepository _negotiationRepository;

    public NegotiationItemService(
        INegotiationItemRepository negotiationItemRepository,
        INegotiationRepository negotiationRepository)
    {
        _negotiationItemRepository = negotiationItemRepository ?? throw new ArgumentNullException(nameof(negotiationItemRepository));
        _negotiationRepository = negotiationRepository ?? throw new ArgumentNullException(nameof(negotiationRepository));
    }

    public async Task<NegotiationItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiationItem = await _negotiationItemRepository.GetByIdAsync(id, cancellationToken);
        if (negotiationItem == null)
            throw new ArgumentException($"NegotiationItem with id {id} not found", nameof(id));

        return MapToDto(negotiationItem);
    }

    public async Task<IEnumerable<NegotiationItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var negotiationItems = await _negotiationItemRepository.GetAllAsync(cancellationToken);
        return negotiationItems.Select(MapToDto);
    }

    public async Task<IEnumerable<NegotiationItemDto>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default)
    {
        var negotiationItems = await _negotiationItemRepository.GetByNegotiationIdAsync(negotiationId, cancellationToken);
        return negotiationItems.Select(MapToDto);
    }

    public async Task<NegotiationItemDto> CreateAsync(CreateNegotiationItemDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Validate that the negotiation exists
        var negotiation = await _negotiationRepository.GetByIdAsync(dto.NegotiationId, cancellationToken);
        if (negotiation == null)
            throw new ArgumentException($"Negotiation with id {dto.NegotiationId} not found", nameof(dto.NegotiationId));

        var negotiationItem = new NegotiationItem(
            dto.NegotiationId,
            dto.Type,
            dto.Quantity,
            dto.GrossValueUnit,
            dto.NetValue,
            dto.DiscountValue
        );

        var createdNegotiationItem = await _negotiationItemRepository.CreateAsync(negotiationItem, cancellationToken);
        return MapToDto(createdNegotiationItem);
    }

    public async Task<NegotiationItemDto> UpdateAsync(Guid id, UpdateNegotiationItemDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var negotiationItem = await _negotiationItemRepository.GetByIdAsync(id, cancellationToken);
        if (negotiationItem == null)
            throw new ArgumentException($"NegotiationItem with id {id} not found", nameof(id));

        // Validate that the negotiation still exists
        var negotiation = await _negotiationRepository.GetByIdAsync(negotiationItem.NegotiationId, cancellationToken);
        if (negotiation == null)
            throw new InvalidOperationException($"Cannot update NegotiationItem: Negotiation with id {negotiationItem.NegotiationId} not found");

        negotiationItem.Update(
            dto.Type,
            dto.Quantity,
            dto.GrossValueUnit,
            dto.NetValue,
            dto.DiscountValue
        );

        var updatedNegotiationItem = await _negotiationItemRepository.UpdateAsync(negotiationItem, cancellationToken);
        return MapToDto(updatedNegotiationItem);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiationItem = await _negotiationItemRepository.GetByIdAsync(id, cancellationToken);
        if (negotiationItem == null)
            throw new ArgumentException($"NegotiationItem with id {id} not found", nameof(id));

        return await _negotiationItemRepository.DeleteAsync(id, cancellationToken);
    }

    private static NegotiationItemDto MapToDto(NegotiationItem negotiationItem)
    {
        return new NegotiationItemDto
        {
            Id = negotiationItem.Id,
            NegotiationId = negotiationItem.NegotiationId,
            Type = negotiationItem.Type,
            Quantity = negotiationItem.Quantity,
            GrossValueUnit = negotiationItem.GrossValueUnit,
            GrossValue = negotiationItem.GrossValue,
            NetValue = negotiationItem.NetValue,
            DiscountValue = negotiationItem.DiscountValue,
            CreatedAt = negotiationItem.CreatedAt,
            UpdatedAt = negotiationItem.UpdatedAt
        };
    }
}

