using ApplicationLayer.DTOs.Supply;

namespace ApplicationLayer.Interfaces.Services;

public interface ISupplyService
{
    Task<SupplyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SupplyDto> CreateAsync(CreateSupplyDto dto, CancellationToken cancellationToken = default);
    Task<SupplyDto> UpdateAsync(Guid id, UpdateSupplyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

