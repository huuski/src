using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;

namespace InfrastructureLayer.Repositories;

public class InMemoryNegotiationRepository : INegotiationRepository
{
    private readonly Dictionary<Guid, Negotiation> _negotiations = new();
    private readonly Dictionary<string, Negotiation> _negotiationsByCode = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Guid, List<Negotiation>> _negotiationsByCustomerId = new();
    private readonly Dictionary<Guid, List<Negotiation>> _negotiationsByUserId = new();
    private readonly object _lock = new();

    public InMemoryNegotiationRepository()
    {
        InitializeDefaultNegotiations();
    }

    private void InitializeDefaultNegotiations()
    {
        var customerId1 = new Guid("550e8400-e29b-41d4-a716-446655440001");
        var userId1 = new Guid("850e8400-e29b-41d4-a716-446655440001");

        var negotiation1Id = new Guid("f50e8400-e29b-41d4-a716-446655440001");
        var negotiation1 = new Negotiation(
            customerId1,
            userId1,
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            500.00m,
            450.00m,
            50.00m
        );
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(negotiation1, negotiation1Id);
        }
        _negotiations[negotiation1.Id] = negotiation1;
        AddToIndexes(negotiation1);

        var negotiation2Id = new Guid("f50e8400-e29b-41d4-a716-446655440002");
        var negotiation2 = new Negotiation(
            new Guid("550e8400-e29b-41d4-a716-446655440002"),
            userId1,
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            300.00m,
            270.00m,
            30.00m
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(negotiation2, negotiation2Id);
        }
        _negotiations[negotiation2.Id] = negotiation2;
        AddToIndexes(negotiation2);
    }

    private void AddToIndexes(Negotiation negotiation)
    {
        _negotiationsByCode[negotiation.Code] = negotiation;

        if (!_negotiationsByCustomerId.TryGetValue(negotiation.CustomerId, out var customerNegotiations))
        {
            customerNegotiations = new List<Negotiation>();
            _negotiationsByCustomerId[negotiation.CustomerId] = customerNegotiations;
        }
        customerNegotiations.Add(negotiation);

        if (!_negotiationsByUserId.TryGetValue(negotiation.UserId, out var userNegotiations))
        {
            userNegotiations = new List<Negotiation>();
            _negotiationsByUserId[negotiation.UserId] = userNegotiations;
        }
        userNegotiations.Add(negotiation);
    }

    public Task<Negotiation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _negotiations.TryGetValue(id, out var negotiation);
            return Task.FromResult<Negotiation?>(negotiation?.IsDeleted == false ? negotiation : null);
        }
    }

    public Task<Negotiation?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _negotiationsByCode.TryGetValue(code, out var negotiation);
            return Task.FromResult<Negotiation?>(negotiation?.IsDeleted == false ? negotiation : null);
        }
    }

    public Task<IEnumerable<Negotiation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Negotiation>>(
                _negotiations.Values.Where(n => !n.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<Negotiation>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_negotiationsByCustomerId.TryGetValue(customerId, out var negotiations))
                return Task.FromResult<IEnumerable<Negotiation>>(Enumerable.Empty<Negotiation>());

            return Task.FromResult<IEnumerable<Negotiation>>(
                negotiations.Where(n => !n.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<Negotiation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_negotiationsByUserId.TryGetValue(userId, out var negotiations))
                return Task.FromResult<IEnumerable<Negotiation>>(Enumerable.Empty<Negotiation>());

            return Task.FromResult<IEnumerable<Negotiation>>(
                negotiations.Where(n => !n.IsDeleted).ToList()
            );
        }
    }

    public Task<Negotiation> CreateAsync(Negotiation negotiation, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_negotiations.ContainsKey(negotiation.Id))
                throw new InvalidOperationException($"Negotiation with id {negotiation.Id} already exists");

            if (_negotiationsByCode.ContainsKey(negotiation.Code))
                throw new InvalidOperationException($"Negotiation with code {negotiation.Code} already exists");

            _negotiations[negotiation.Id] = negotiation;
            _negotiationsByCode[negotiation.Code] = negotiation;

            if (!_negotiationsByCustomerId.TryGetValue(negotiation.CustomerId, out var customerNegotiations))
            {
                customerNegotiations = new List<Negotiation>();
                _negotiationsByCustomerId[negotiation.CustomerId] = customerNegotiations;
            }
            customerNegotiations.Add(negotiation);

            if (!_negotiationsByUserId.TryGetValue(negotiation.UserId, out var userNegotiations))
            {
                userNegotiations = new List<Negotiation>();
                _negotiationsByUserId[negotiation.UserId] = userNegotiations;
            }
            userNegotiations.Add(negotiation);

            return Task.FromResult(negotiation);
        }
    }

    public Task<Negotiation> UpdateAsync(Negotiation negotiation, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_negotiations.ContainsKey(negotiation.Id))
                throw new InvalidOperationException($"Negotiation with id {negotiation.Id} not found");

            var existingNegotiation = _negotiations[negotiation.Id];
            if (existingNegotiation.Code != negotiation.Code)
            {
                _negotiationsByCode.Remove(existingNegotiation.Code);
                _negotiationsByCode[negotiation.Code] = negotiation;
            }

            _negotiations[negotiation.Id] = negotiation;
            return Task.FromResult(negotiation);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiation = await GetByIdAsync(id, cancellationToken);
        if (negotiation == null)
            return false;

        lock (_lock)
        {
            negotiation.MarkAsDeleted();
            _negotiations[negotiation.Id] = negotiation;
            return true;
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _negotiations.Clear();
            _negotiationsByCode.Clear();
            _negotiationsByCustomerId.Clear();
            _negotiationsByUserId.Clear();
            InitializeDefaultNegotiations();
        }
    }
}

