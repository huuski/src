using ApplicationLayer.DTOs.PaymentMethod;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;

namespace ApplicationLayer.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository ?? throw new ArgumentNullException(nameof(paymentMethodRepository));
    }

    public async Task<PaymentMethodDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (paymentMethod == null)
            throw new ArgumentException($"PaymentMethod with id {id} not found", nameof(id));

        return MapToDto(paymentMethod);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var paymentMethods = await _paymentMethodRepository.GetAllAsync(cancellationToken);
        return paymentMethods.Select(MapToDto);
    }

    public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var paymentMethod = new PaymentMethod(
            dto.Name,
            dto.Type
        );

        var createdPaymentMethod = await _paymentMethodRepository.CreateAsync(paymentMethod, cancellationToken);
        return MapToDto(createdPaymentMethod);
    }

    public async Task<PaymentMethodDto> UpdateAsync(Guid id, UpdatePaymentMethodDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (paymentMethod == null)
            throw new ArgumentException($"PaymentMethod with id {id} not found", nameof(id));

        paymentMethod.Update(
            dto.Name,
            dto.Type
        );

        var updatedPaymentMethod = await _paymentMethodRepository.UpdateAsync(paymentMethod, cancellationToken);
        return MapToDto(updatedPaymentMethod);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (paymentMethod == null)
            throw new ArgumentException($"PaymentMethod with id {id} not found", nameof(id));

        return await _paymentMethodRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<PaymentMethodDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (paymentMethod == null)
            throw new ArgumentException($"PaymentMethod with id {id} not found", nameof(id));

        paymentMethod.Activate();
        var updatedPaymentMethod = await _paymentMethodRepository.UpdateAsync(paymentMethod, cancellationToken);
        return MapToDto(updatedPaymentMethod);
    }

    public async Task<PaymentMethodDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, cancellationToken);
        if (paymentMethod == null)
            throw new ArgumentException($"PaymentMethod with id {id} not found", nameof(id));

        paymentMethod.Deactivate();
        var updatedPaymentMethod = await _paymentMethodRepository.UpdateAsync(paymentMethod, cancellationToken);
        return MapToDto(updatedPaymentMethod);
    }

    private static PaymentMethodDto MapToDto(PaymentMethod paymentMethod)
    {
        return new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            Name = paymentMethod.Name,
            Type = paymentMethod.Type,
            Inactive = paymentMethod.Inactive,
            CreatedAt = paymentMethod.CreatedAt,
            UpdatedAt = paymentMethod.UpdatedAt
        };
    }
}

