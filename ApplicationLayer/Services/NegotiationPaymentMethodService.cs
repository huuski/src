using ApplicationLayer.DTOs.NegotiationPaymentMethod;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class NegotiationPaymentMethodService : INegotiationPaymentMethodService
{
    private readonly INegotiationPaymentMethodRepository _negotiationPaymentMethodRepository;

    public NegotiationPaymentMethodService(INegotiationPaymentMethodRepository negotiationPaymentMethodRepository)
    {
        _negotiationPaymentMethodRepository = negotiationPaymentMethodRepository ?? throw new ArgumentNullException(nameof(negotiationPaymentMethodRepository));
    }

    public async Task<NegotiationPaymentMethodDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiationPaymentMethod = await _negotiationPaymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (negotiationPaymentMethod == null)
            throw new ArgumentException($"NegotiationPaymentMethod with id {id} not found", nameof(id));

        return MapToDto(negotiationPaymentMethod);
    }

    public async Task<IEnumerable<NegotiationPaymentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var negotiationPaymentMethods = await _negotiationPaymentMethodRepository.GetAllAsync(cancellationToken);
        return negotiationPaymentMethods.Select(MapToDto);
    }

    public async Task<IEnumerable<NegotiationPaymentMethodDto>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default)
    {
        var negotiationPaymentMethods = await _negotiationPaymentMethodRepository.GetByNegotiationIdAsync(negotiationId, cancellationToken);
        return negotiationPaymentMethods.Select(MapToDto);
    }

    public async Task<NegotiationPaymentMethodDto> CreateAsync(CreateNegotiationPaymentMethodDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var negotiationPaymentMethod = new NegotiationPaymentMethod(
            dto.NegotiationId,
            dto.PaymentMethodId,
            dto.Value
        );

        var createdNegotiationPaymentMethod = await _negotiationPaymentMethodRepository.CreateAsync(negotiationPaymentMethod, cancellationToken);
        return MapToDto(createdNegotiationPaymentMethod);
    }

    public async Task<NegotiationPaymentMethodDto> UpdateAsync(Guid id, UpdateNegotiationPaymentMethodDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var negotiationPaymentMethod = await _negotiationPaymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (negotiationPaymentMethod == null)
            throw new ArgumentException($"NegotiationPaymentMethod with id {id} not found", nameof(id));

        negotiationPaymentMethod.Update(dto.Value);

        var updatedNegotiationPaymentMethod = await _negotiationPaymentMethodRepository.UpdateAsync(negotiationPaymentMethod, cancellationToken);
        return MapToDto(updatedNegotiationPaymentMethod);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiationPaymentMethod = await _negotiationPaymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (negotiationPaymentMethod == null)
            throw new ArgumentException($"NegotiationPaymentMethod with id {id} not found", nameof(id));

        return await _negotiationPaymentMethodRepository.DeleteAsync(id, cancellationToken);
    }

    private static NegotiationPaymentMethodDto MapToDto(NegotiationPaymentMethod negotiationPaymentMethod)
    {
        return new NegotiationPaymentMethodDto
        {
            Id = negotiationPaymentMethod.Id,
            NegotiationId = negotiationPaymentMethod.NegotiationId,
            PaymentMethodId = negotiationPaymentMethod.PaymentMethodId,
            Value = negotiationPaymentMethod.Value,
            CreatedAt = negotiationPaymentMethod.CreatedAt,
            UpdatedAt = negotiationPaymentMethod.UpdatedAt
        };
    }
}

