using ApplicationLayer.DTOs.Supply;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class SupplyService : ISupplyService
{
    private readonly ISupplyRepository _supplyRepository;

    public SupplyService(ISupplyRepository supplyRepository)
    {
        _supplyRepository = supplyRepository ?? throw new ArgumentNullException(nameof(supplyRepository));
    }

    public async Task<SupplyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supply = await _supplyRepository.GetByIdAsync(id, cancellationToken);
        if (supply == null)
            throw new ArgumentException($"Supply with id {id} not found", nameof(id));

        return MapToDto(supply);
    }

    public async Task<IEnumerable<SupplyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var supplies = await _supplyRepository.GetAllAsync(cancellationToken);
        return supplies.Select(MapToDto);
    }

    public async Task<SupplyDto> CreateAsync(CreateSupplyDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var supply = new Supply(dto.Name, dto.Stock);

        var createdSupply = await _supplyRepository.CreateAsync(supply, cancellationToken);
        return MapToDto(createdSupply);
    }

    public async Task<SupplyDto> UpdateAsync(Guid id, UpdateSupplyDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var supply = await _supplyRepository.GetByIdAsync(id, cancellationToken);
        if (supply == null)
            throw new ArgumentException($"Supply with id {id} not found", nameof(id));

        supply.Update(dto.Name, dto.Stock);

        var updatedSupply = await _supplyRepository.UpdateAsync(supply, cancellationToken);
        return MapToDto(updatedSupply);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supply = await _supplyRepository.GetByIdAsync(id, cancellationToken);
        if (supply == null)
            throw new ArgumentException($"Supply with id {id} not found", nameof(id));

        return await _supplyRepository.DeleteAsync(id, cancellationToken);
    }

    private static SupplyDto MapToDto(Supply supply)
    {
        return new SupplyDto
        {
            Id = supply.Id,
            Name = supply.Name,
            Stock = supply.Stock,
            CreatedAt = supply.CreatedAt,
            UpdatedAt = supply.UpdatedAt
        };
    }
}

