using ApplicationLayer.DTOs.Voucher;

namespace ApplicationLayer.Interfaces.Services;

public interface IVoucherService
{
    Task<VoucherDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VoucherDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<VoucherDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<VoucherDto> CreateAsync(CreateVoucherDto dto, CancellationToken cancellationToken = default);
    Task<VoucherDto> UpdateAsync(Guid id, UpdateVoucherDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

