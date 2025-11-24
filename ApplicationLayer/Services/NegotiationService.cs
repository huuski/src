using ApplicationLayer.DTOs.Negotiation;
using ApplicationLayer.DTOs.NegotiationItem;
using ApplicationLayer.DTOs.NegotiationPaymentMethod;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class NegotiationService : INegotiationService
{
    private readonly INegotiationRepository _negotiationRepository;
    private readonly INegotiationItemRepository _negotiationItemRepository;
    private readonly INegotiationPaymentMethodRepository _negotiationPaymentMethodRepository;

    public NegotiationService(
        INegotiationRepository negotiationRepository,
        INegotiationItemRepository negotiationItemRepository,
        INegotiationPaymentMethodRepository negotiationPaymentMethodRepository)
    {
        _negotiationRepository = negotiationRepository ?? throw new ArgumentNullException(nameof(negotiationRepository));
        _negotiationItemRepository = negotiationItemRepository ?? throw new ArgumentNullException(nameof(negotiationItemRepository));
        _negotiationPaymentMethodRepository = negotiationPaymentMethodRepository ?? throw new ArgumentNullException(nameof(negotiationPaymentMethodRepository));
    }

    public async Task<NegotiationDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiation = await _negotiationRepository.GetByIdAsync(id, cancellationToken);
        if (negotiation == null)
            throw new ArgumentException($"Negotiation with id {id} not found", nameof(id));

        return MapToDto(negotiation);
    }

    public async Task<NegotiationCompleteDto> GetByIdCompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiation = await _negotiationRepository.GetByIdAsync(id, cancellationToken);
        if (negotiation == null)
            throw new ArgumentException($"Negotiation with id {id} not found", nameof(id));

        var items = await _negotiationItemRepository.GetByNegotiationIdAsync(id, cancellationToken);
        var paymentMethods = await _negotiationPaymentMethodRepository.GetByNegotiationIdAsync(id, cancellationToken);
        
        return MapToCompleteDto(negotiation, items, paymentMethods);
    }

    public async Task<NegotiationWithItemsDto> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiation = await _negotiationRepository.GetByIdAsync(id, cancellationToken);
        if (negotiation == null)
            throw new ArgumentException($"Negotiation with id {id} not found", nameof(id));

        var items = await _negotiationItemRepository.GetByNegotiationIdAsync(id, cancellationToken);
        return MapToDtoWithItems(negotiation, items);
    }

    public async Task<NegotiationDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        var negotiation = await _negotiationRepository.GetByCodeAsync(code, cancellationToken);
        return negotiation != null ? MapToDto(negotiation) : null;
    }

    public async Task<NegotiationWithItemsDto?> GetByCodeWithItemsAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        var negotiation = await _negotiationRepository.GetByCodeAsync(code, cancellationToken);
        if (negotiation == null)
            return null;

        var items = await _negotiationItemRepository.GetByNegotiationIdAsync(negotiation.Id, cancellationToken);
        return MapToDtoWithItems(negotiation, items);
    }

    public async Task<IEnumerable<NegotiationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var negotiations = await _negotiationRepository.GetAllAsync(cancellationToken);
        return negotiations.Select(MapToDto);
    }

    public async Task<IEnumerable<NegotiationDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var negotiations = await _negotiationRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return negotiations.Select(MapToDto);
    }

    public async Task<IEnumerable<NegotiationDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var negotiations = await _negotiationRepository.GetByUserIdAsync(userId, cancellationToken);
        return negotiations.Select(MapToDto);
    }

    public async Task<NegotiationDto> CreateAsync(CreateNegotiationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Validate UserId explicitly
        if (dto.UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(dto.UserId));

        var negotiation = new Negotiation(
            dto.CustomerId,
            dto.UserId,
            dto.ExpirationDate,
            dto.GrossValue,
            dto.NetValue,
            dto.DiscountValue
        );

        var createdNegotiation = await _negotiationRepository.CreateAsync(negotiation, cancellationToken);

        // Create negotiation items if provided
        if (dto.Items != null && dto.Items.Any())
        {
            foreach (var itemDto in dto.Items)
            {
                var negotiationItem = new NegotiationItem(
                    createdNegotiation.Id,
                    itemDto.Type,
                    itemDto.Quantity,
                    itemDto.GrossValueUnit,
                    itemDto.NetValue,
                    itemDto.DiscountValue
                );

                await _negotiationItemRepository.CreateAsync(negotiationItem, cancellationToken);
            }
        }

        return MapToDto(createdNegotiation);
    }

    public async Task<NegotiationDto> UpdateAsync(Guid id, UpdateNegotiationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var negotiation = await _negotiationRepository.GetByIdAsync(id, cancellationToken);
        if (negotiation == null)
            throw new ArgumentException($"Negotiation with id {id} not found", nameof(id));

        negotiation.Update(
            dto.ExpirationDate,
            dto.GrossValue,
            dto.NetValue,
            dto.DiscountValue
        );

        var updatedNegotiation = await _negotiationRepository.UpdateAsync(negotiation, cancellationToken);
        return MapToDto(updatedNegotiation);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiation = await _negotiationRepository.GetByIdAsync(id, cancellationToken);
        if (negotiation == null)
            throw new ArgumentException($"Negotiation with id {id} not found", nameof(id));

        return await _negotiationRepository.DeleteAsync(id, cancellationToken);
    }

    private static NegotiationDto MapToDto(Negotiation negotiation)
    {
        return new NegotiationDto
        {
            Id = negotiation.Id,
            CustomerId = negotiation.CustomerId,
            UserId = negotiation.UserId,
            Code = negotiation.Code,
            ExpirationDate = negotiation.ExpirationDate,
            GrossValue = negotiation.GrossValue,
            NetValue = negotiation.NetValue,
            DiscountValue = negotiation.DiscountValue,
            CreatedAt = negotiation.CreatedAt,
            UpdatedAt = negotiation.UpdatedAt
        };
    }

    private static NegotiationWithItemsDto MapToDtoWithItems(Negotiation negotiation, IEnumerable<NegotiationItem> items)
    {
        return new NegotiationWithItemsDto
        {
            Id = negotiation.Id,
            CustomerId = negotiation.CustomerId,
            UserId = negotiation.UserId,
            Code = negotiation.Code,
            ExpirationDate = negotiation.ExpirationDate,
            GrossValue = negotiation.GrossValue,
            NetValue = negotiation.NetValue,
            DiscountValue = negotiation.DiscountValue,
            CreatedAt = negotiation.CreatedAt,
            UpdatedAt = negotiation.UpdatedAt,
            Items = items.Select(MapItemToDto).ToList()
        };
    }

    private static NegotiationItemDto MapItemToDto(NegotiationItem item)
    {
        return new NegotiationItemDto
        {
            Id = item.Id,
            NegotiationId = item.NegotiationId,
            Type = item.Type,
            Quantity = item.Quantity,
            GrossValueUnit = item.GrossValueUnit,
            GrossValue = item.GrossValue,
            NetValue = item.NetValue,
            DiscountValue = item.DiscountValue,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    private static NegotiationCompleteDto MapToCompleteDto(
        Negotiation negotiation,
        IEnumerable<NegotiationItem> items,
        IEnumerable<NegotiationPaymentMethod> paymentMethods)
    {
        return new NegotiationCompleteDto
        {
            Id = negotiation.Id,
            CustomerId = negotiation.CustomerId,
            UserId = negotiation.UserId,
            Code = negotiation.Code,
            ExpirationDate = negotiation.ExpirationDate,
            GrossValue = negotiation.GrossValue,
            NetValue = negotiation.NetValue,
            DiscountValue = negotiation.DiscountValue,
            CreatedAt = negotiation.CreatedAt,
            UpdatedAt = negotiation.UpdatedAt,
            Items = items.Select(MapItemToDto).ToList(),
            PaymentMethods = paymentMethods.Select(MapPaymentMethodToDto).ToList()
        };
    }

    private static NegotiationPaymentMethodDto MapPaymentMethodToDto(NegotiationPaymentMethod paymentMethod)
    {
        return new NegotiationPaymentMethodDto
        {
            Id = paymentMethod.Id,
            NegotiationId = paymentMethod.NegotiationId,
            PaymentMethodId = paymentMethod.PaymentMethodId,
            Value = paymentMethod.Value,
            CreatedAt = paymentMethod.CreatedAt,
            UpdatedAt = paymentMethod.UpdatedAt
        };
    }
}

