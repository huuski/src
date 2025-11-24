using ApplicationLayer.DTOs.Negotiation;

namespace ApplicationLayer.Interfaces.Services;

public interface INegotiationService
{
    Task<NegotiationDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NegotiationWithItemsDto> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NegotiationCompleteDto> GetByIdCompleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NegotiationDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<NegotiationWithItemsDto?> GetByCodeWithItemsAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NegotiationDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<NegotiationDto> CreateAsync(CreateNegotiationDto dto, CancellationToken cancellationToken = default);
    Task<NegotiationDto> UpdateAsync(Guid id, UpdateNegotiationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

