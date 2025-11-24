using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemorySupplyRepository : ISupplyRepository
{
    private readonly Dictionary<Guid, Supply> _supplies = new();
    private readonly object _lock = new();

    public InMemorySupplyRepository(SeedDataService? seedDataService = null)
    {
        InitializeDefaultSupplies();
    }

    private void InitializeDefaultSupplies()
    {
        var idProperty = typeof(Entity).GetProperty("Id");

        // Supply 1: Luvas
        var supply1Id = new Guid("d50e8400-e29b-41d4-a716-446655440001");
        var supply1 = new Supply("Luvas", 200);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(supply1, supply1Id);
        }
        _supplies[supply1.Id] = supply1;

        // Supply 2: Algodão
        var supply2Id = new Guid("d50e8400-e29b-41d4-a716-446655440002");
        var supply2 = new Supply("Algodão", 150);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(supply2, supply2Id);
        }
        _supplies[supply2.Id] = supply2;

        // Supply 3: Gaze
        var supply3Id = new Guid("d50e8400-e29b-41d4-a716-446655440003");
        var supply3 = new Supply("Gaze", 100);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(supply3, supply3Id);
        }
        _supplies[supply3.Id] = supply3;
    }

    public Task<Supply?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _supplies.TryGetValue(id, out var supply);
            return Task.FromResult<Supply?>(supply?.IsDeleted == false ? supply : null);
        }
    }

    public Task<IEnumerable<Supply>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Supply>>(
                _supplies.Values.Where(s => !s.IsDeleted).ToList()
            );
        }
    }

    public Task<Supply> CreateAsync(Supply supply, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_supplies.ContainsKey(supply.Id))
                throw new InvalidOperationException($"Supply with id {supply.Id} already exists");

            _supplies[supply.Id] = supply;
            return Task.FromResult(supply);
        }
    }

    public Task<Supply> UpdateAsync(Supply supply, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_supplies.ContainsKey(supply.Id))
                throw new InvalidOperationException($"Supply with id {supply.Id} not found");

            _supplies[supply.Id] = supply;
            return Task.FromResult(supply);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supply = await GetByIdAsync(id, cancellationToken);
        if (supply == null)
            return false;

        lock (_lock)
        {
            supply.MarkAsDeleted();
            _supplies[supply.Id] = supply;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _supplies.Clear();
            InitializeDefaultSupplies();
        }
    }
}

