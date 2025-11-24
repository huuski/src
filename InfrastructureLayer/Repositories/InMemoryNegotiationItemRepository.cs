using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;

namespace InfrastructureLayer.Repositories;

public class InMemoryNegotiationItemRepository : INegotiationItemRepository
{
    private readonly Dictionary<Guid, NegotiationItem> _negotiationItems = new();
    private readonly Dictionary<Guid, List<NegotiationItem>> _itemsByNegotiationId = new();
    private readonly INegotiationRepository? _negotiationRepository;
    private readonly object _lock = new();

    public InMemoryNegotiationItemRepository(INegotiationRepository? negotiationRepository = null)
    {
        _negotiationRepository = negotiationRepository;
        InitializeDefaultNegotiationItems();
    }

    private void InitializeDefaultNegotiationItems()
    {
        var negotiationId1 = new Guid("f50e8400-e29b-41d4-a716-446655440001");
        
        var item1 = new NegotiationItem(
            negotiationId1,
            DomainLayer.Enums.NegotiationItemType.Service,
            1,
            250.00m,
            225.00m,
            25.00m
        );
        _negotiationItems[item1.Id] = item1;
        AddToIndex(item1);

        var item2 = new NegotiationItem(
            negotiationId1,
            DomainLayer.Enums.NegotiationItemType.Product,
            2,
            125.00m,
            225.00m,
            25.00m
        );
        _negotiationItems[item2.Id] = item2;
        AddToIndex(item2);
    }

    private void AddToIndex(NegotiationItem item)
    {
        if (!_itemsByNegotiationId.TryGetValue(item.NegotiationId, out var items))
        {
            items = new List<NegotiationItem>();
            _itemsByNegotiationId[item.NegotiationId] = items;
        }
        items.Add(item);
    }

    public Task<NegotiationItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _negotiationItems.TryGetValue(id, out var negotiationItem);
            return Task.FromResult<NegotiationItem?>(negotiationItem?.IsDeleted == false ? negotiationItem : null);
        }
    }

    public Task<IEnumerable<NegotiationItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<NegotiationItem>>(
                _negotiationItems.Values.Where(ni => !ni.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<NegotiationItem>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_itemsByNegotiationId.TryGetValue(negotiationId, out var items))
                return Task.FromResult<IEnumerable<NegotiationItem>>(Enumerable.Empty<NegotiationItem>());

            return Task.FromResult<IEnumerable<NegotiationItem>>(
                items.Where(i => !i.IsDeleted).ToList()
            );
        }
    }

    public async Task<NegotiationItem> CreateAsync(NegotiationItem negotiationItem, CancellationToken cancellationToken = default)
    {
        // Validate that the negotiation exists if repository is available
        if (_negotiationRepository != null)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(negotiationItem.NegotiationId, cancellationToken);
            if (negotiation == null)
                throw new InvalidOperationException($"Cannot create NegotiationItem: Negotiation with id {negotiationItem.NegotiationId} not found");
        }

        lock (_lock)
        {
            if (_negotiationItems.ContainsKey(negotiationItem.Id))
                throw new InvalidOperationException($"NegotiationItem with id {negotiationItem.Id} already exists");

            _negotiationItems[negotiationItem.Id] = negotiationItem;

            if (!_itemsByNegotiationId.TryGetValue(negotiationItem.NegotiationId, out var items))
            {
                items = new List<NegotiationItem>();
                _itemsByNegotiationId[negotiationItem.NegotiationId] = items;
            }
            items.Add(negotiationItem);
        }

        return negotiationItem;
    }

    public Task<NegotiationItem> UpdateAsync(NegotiationItem negotiationItem, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_negotiationItems.ContainsKey(negotiationItem.Id))
                throw new InvalidOperationException($"NegotiationItem with id {negotiationItem.Id} not found");

            var existingItem = _negotiationItems[negotiationItem.Id];
            if (existingItem.NegotiationId != negotiationItem.NegotiationId)
            {
                // Remove from old negotiation
                if (_itemsByNegotiationId.TryGetValue(existingItem.NegotiationId, out var oldItems))
                {
                    oldItems.Remove(existingItem);
                }

                // Add to new negotiation
                if (!_itemsByNegotiationId.TryGetValue(negotiationItem.NegotiationId, out var newItems))
                {
                    newItems = new List<NegotiationItem>();
                    _itemsByNegotiationId[negotiationItem.NegotiationId] = newItems;
                }
                newItems.Add(negotiationItem);
            }

            _negotiationItems[negotiationItem.Id] = negotiationItem;
            return Task.FromResult(negotiationItem);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiationItem = await GetByIdAsync(id, cancellationToken);
        if (negotiationItem == null)
            return false;

        lock (_lock)
        {
            negotiationItem.MarkAsDeleted();
            _negotiationItems[negotiationItem.Id] = negotiationItem;
            return true;
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _negotiationItems.Clear();
            _itemsByNegotiationId.Clear();
            InitializeDefaultNegotiationItems();
        }
    }
}

