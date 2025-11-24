using ApplicationLayer.DTOs.SpotlightCard;

namespace ApplicationLayer.Interfaces.Services;

public interface ISpotlightCardService
{
    Task<SpotlightCardDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SpotlightCardDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SpotlightCardDto> CreateAsync(CreateSpotlightCardDto dto, CancellationToken cancellationToken = default);
    Task<SpotlightCardDto> UpdateAsync(Guid id, UpdateSpotlightCardDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SpotlightCardDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SpotlightCardDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}

