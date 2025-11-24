using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IVoucherRepository
{
    Task<Voucher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Voucher?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Voucher>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Voucher> CreateAsync(Voucher voucher, CancellationToken cancellationToken = default);
    Task<Voucher> UpdateAsync(Voucher voucher, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

