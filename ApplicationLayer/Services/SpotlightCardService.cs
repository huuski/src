using ApplicationLayer.DTOs.SpotlightCard;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class SpotlightCardService : ISpotlightCardService
{
    private readonly ISpotlightCardRepository _spotlightCardRepository;

    public SpotlightCardService(ISpotlightCardRepository spotlightCardRepository)
    {
        _spotlightCardRepository = spotlightCardRepository ?? throw new ArgumentNullException(nameof(spotlightCardRepository));
    }

    public async Task<SpotlightCardDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spotlightCard = await _spotlightCardRepository.GetByIdAsync(id, cancellationToken);
        if (spotlightCard == null)
            throw new ArgumentException($"SpotlightCard with id {id} not found", nameof(id));

        return MapToDto(spotlightCard);
    }

    public async Task<IEnumerable<SpotlightCardDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var spotlightCards = await _spotlightCardRepository.GetAllAsync(cancellationToken);
        return spotlightCards.Select(MapToDto);
    }

    public async Task<SpotlightCardDto> CreateAsync(CreateSpotlightCardDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var spotlightCard = new SpotlightCard(
            dto.Title,
            dto.ShortDescription,
            dto.LongDescription,
            dto.InitDate,
            dto.EndDate,
            dto.Image,
            dto.ButtonTitle,
            dto.ButtonLink
        );

        var createdSpotlightCard = await _spotlightCardRepository.CreateAsync(spotlightCard, cancellationToken);
        return MapToDto(createdSpotlightCard);
    }

    public async Task<SpotlightCardDto> UpdateAsync(Guid id, UpdateSpotlightCardDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var spotlightCard = await _spotlightCardRepository.GetByIdAsync(id, cancellationToken);
        if (spotlightCard == null)
            throw new ArgumentException($"SpotlightCard with id {id} not found", nameof(id));

        spotlightCard.Update(
            dto.Title,
            dto.ShortDescription,
            dto.LongDescription,
            dto.InitDate,
            dto.EndDate,
            dto.Image,
            dto.ButtonTitle,
            dto.ButtonLink
        );

        var updatedSpotlightCard = await _spotlightCardRepository.UpdateAsync(spotlightCard, cancellationToken);
        return MapToDto(updatedSpotlightCard);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spotlightCard = await _spotlightCardRepository.GetByIdAsync(id, cancellationToken);
        if (spotlightCard == null)
            throw new ArgumentException($"SpotlightCard with id {id} not found", nameof(id));

        return await _spotlightCardRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<SpotlightCardDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spotlightCard = await _spotlightCardRepository.GetByIdAsync(id, cancellationToken);
        if (spotlightCard == null)
            throw new ArgumentException($"SpotlightCard with id {id} not found", nameof(id));

        spotlightCard.Activate();
        var updatedSpotlightCard = await _spotlightCardRepository.UpdateAsync(spotlightCard, cancellationToken);
        return MapToDto(updatedSpotlightCard);
    }

    public async Task<SpotlightCardDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spotlightCard = await _spotlightCardRepository.GetByIdAsync(id, cancellationToken);
        if (spotlightCard == null)
            throw new ArgumentException($"SpotlightCard with id {id} not found", nameof(id));

        spotlightCard.Deactivate();
        var updatedSpotlightCard = await _spotlightCardRepository.UpdateAsync(spotlightCard, cancellationToken);
        return MapToDto(updatedSpotlightCard);
    }

    private static SpotlightCardDto MapToDto(SpotlightCard spotlightCard)
    {
        return new SpotlightCardDto
        {
            Id = spotlightCard.Id,
            Title = spotlightCard.Title,
            ShortDescription = spotlightCard.ShortDescription,
            LongDescription = spotlightCard.LongDescription,
            Image = spotlightCard.Image,
            ButtonTitle = spotlightCard.ButtonTitle,
            ButtonLink = spotlightCard.ButtonLink,
            InitDate = spotlightCard.InitDate,
            EndDate = spotlightCard.EndDate,
            Inactive = spotlightCard.Inactive,
            IsActive = spotlightCard.IsActive,
            CreatedAt = spotlightCard.CreatedAt,
            UpdatedAt = spotlightCard.UpdatedAt
        };
    }
}

