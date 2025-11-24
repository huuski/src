using ApplicationLayer.DTOs.NegotiationItem;

namespace ApplicationLayer.Interfaces.Services;

public interface INegotiationItemService
{
    Task<NegotiationItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationItemDto>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default);
    Task<NegotiationItemDto> CreateAsync(CreateNegotiationItemDto dto, CancellationToken cancellationToken = default);
    Task<NegotiationItemDto> UpdateAsync(Guid id, UpdateNegotiationItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

