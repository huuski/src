using ApplicationLayer.DTOs.Voucher;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;

    public VoucherService(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
    }

    public async Task<VoucherDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id, cancellationToken);
        if (voucher == null)
            throw new ArgumentException($"Voucher with id {id} not found", nameof(id));

        return MapToDto(voucher);
    }

    public async Task<VoucherDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        var voucher = await _voucherRepository.GetByCodeAsync(code, cancellationToken);
        return voucher != null ? MapToDto(voucher) : null;
    }

    public async Task<IEnumerable<VoucherDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var vouchers = await _voucherRepository.GetAllAsync(cancellationToken);
        return vouchers.Select(MapToDto);
    }

    public async Task<VoucherDto> CreateAsync(CreateVoucherDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var voucher = new Voucher(
            dto.Code,
            dto.Description,
            dto.DiscountAmount,
            dto.ValidFrom,
            dto.ValidUntil,
            dto.IsActive,
            dto.MinimumPurchaseAmount
        );

        var createdVoucher = await _voucherRepository.CreateAsync(voucher, cancellationToken);
        return MapToDto(createdVoucher);
    }

    public async Task<VoucherDto> UpdateAsync(Guid id, UpdateVoucherDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var voucher = await _voucherRepository.GetByIdAsync(id, cancellationToken);
        if (voucher == null)
            throw new ArgumentException($"Voucher with id {id} not found", nameof(id));

        voucher.Update(
            dto.Code,
            dto.Description,
            dto.DiscountAmount,
            dto.ValidFrom,
            dto.ValidUntil,
            dto.IsActive,
            dto.MinimumPurchaseAmount
        );

        var updatedVoucher = await _voucherRepository.UpdateAsync(voucher, cancellationToken);
        return MapToDto(updatedVoucher);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id, cancellationToken);
        if (voucher == null)
            throw new ArgumentException($"Voucher with id {id} not found", nameof(id));

        return await _voucherRepository.DeleteAsync(id, cancellationToken);
    }

    private static VoucherDto MapToDto(Voucher voucher)
    {
        return new VoucherDto
        {
            Id = voucher.Id,
            Code = voucher.Code,
            Description = voucher.Description,
            DiscountAmount = voucher.DiscountAmount,
            ValidFrom = voucher.ValidFrom,
            ValidUntil = voucher.ValidUntil,
            IsActive = voucher.IsActive,
            MinimumPurchaseAmount = voucher.MinimumPurchaseAmount,
            CreatedAt = voucher.CreatedAt,
            UpdatedAt = voucher.UpdatedAt
        };
    }
}

