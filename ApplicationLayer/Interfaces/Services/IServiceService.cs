using ApplicationLayer.DTOs.Service;

namespace ApplicationLayer.Interfaces.Services;

public interface IServiceService
{
    Task<ServiceDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken cancellationToken = default);
    Task<ServiceDto> UpdateAsync(Guid id, UpdateServiceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

