using ApplicationLayer.DTOs.Service;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;

namespace ApplicationLayer.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
    }

    public async Task<ServiceDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        if (service == null)
            throw new ArgumentException($"Service with id {id} not found", nameof(id));

        return MapToDto(service);
    }

    public async Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var services = await _serviceRepository.GetAllAsync(cancellationToken);
        return services.Select(MapToDto);
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var service = new Service(
            dto.Name,
            dto.Description,
            dto.Category,
            dto.Amount,
            dto.Duration,
            dto.Image,
            dto.ExecutionFlowId
        );

        var createdService = await _serviceRepository.CreateAsync(service, cancellationToken);
        return MapToDto(createdService);
    }

    public async Task<ServiceDto> UpdateAsync(Guid id, UpdateServiceDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        if (service == null)
            throw new ArgumentException($"Service with id {id} not found", nameof(id));

        service.Update(
            dto.Name,
            dto.Description,
            dto.Category,
            dto.Amount,
            dto.Duration,
            dto.Image,
            dto.ExecutionFlowId
        );

        var updatedService = await _serviceRepository.UpdateAsync(service, cancellationToken);
        return MapToDto(updatedService);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        if (service == null)
            throw new ArgumentException($"Service with id {id} not found", nameof(id));

        return await _serviceRepository.DeleteAsync(id, cancellationToken);
    }

    private static ServiceDto MapToDto(Service service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Category = service.Category,
            Amount = service.Amount,
            Image = service.Image,
            Duration = service.Duration,
            ExecutionFlowId = service.ExecutionFlowId,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }
}

