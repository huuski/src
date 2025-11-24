using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;

namespace InfrastructureLayer.Repositories;

public class InMemoryNegotiationPaymentMethodRepository : INegotiationPaymentMethodRepository
{
    private readonly Dictionary<Guid, NegotiationPaymentMethod> _negotiationPaymentMethods = new();
    private readonly Dictionary<Guid, List<NegotiationPaymentMethod>> _paymentMethodsByNegotiationId = new();
    private readonly object _lock = new();

    public InMemoryNegotiationPaymentMethodRepository()
    {
        InitializeDefaultNegotiationPaymentMethods();
    }

    private void InitializeDefaultNegotiationPaymentMethods()
    {
        var negotiationId1 = new Guid("f50e8400-e29b-41d4-a716-446655440001");
        var paymentMethodId1 = new Guid("350e8400-e29b-41d4-a716-446655440001"); // Cartão de Crédito
        
        var npm1 = new NegotiationPaymentMethod(
            negotiationId1,
            paymentMethodId1,
            450.00m
        );
        _negotiationPaymentMethods[npm1.Id] = npm1;
        AddToIndex(npm1);

        var negotiationId2 = new Guid("f50e8400-e29b-41d4-a716-446655440002");
        var paymentMethodId2 = new Guid("350e8400-e29b-41d4-a716-446655440004"); // PIX
        
        var npm2 = new NegotiationPaymentMethod(
            negotiationId2,
            paymentMethodId2,
            270.00m
        );
        _negotiationPaymentMethods[npm2.Id] = npm2;
        AddToIndex(npm2);
    }

    private void AddToIndex(NegotiationPaymentMethod npm)
    {
        if (!_paymentMethodsByNegotiationId.TryGetValue(npm.NegotiationId, out var paymentMethods))
        {
            paymentMethods = new List<NegotiationPaymentMethod>();
            _paymentMethodsByNegotiationId[npm.NegotiationId] = paymentMethods;
        }
        paymentMethods.Add(npm);
    }

    public Task<NegotiationPaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _negotiationPaymentMethods.TryGetValue(id, out var negotiationPaymentMethod);
            return Task.FromResult<NegotiationPaymentMethod?>(negotiationPaymentMethod?.IsDeleted == false ? negotiationPaymentMethod : null);
        }
    }

    public Task<IEnumerable<NegotiationPaymentMethod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<NegotiationPaymentMethod>>(
                _negotiationPaymentMethods.Values.Where(npm => !npm.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<NegotiationPaymentMethod>> GetByNegotiationIdAsync(Guid negotiationId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_paymentMethodsByNegotiationId.TryGetValue(negotiationId, out var paymentMethods))
                return Task.FromResult<IEnumerable<NegotiationPaymentMethod>>(Enumerable.Empty<NegotiationPaymentMethod>());

            return Task.FromResult<IEnumerable<NegotiationPaymentMethod>>(
                paymentMethods.Where(pm => !pm.IsDeleted).ToList()
            );
        }
    }

    public Task<NegotiationPaymentMethod> CreateAsync(NegotiationPaymentMethod negotiationPaymentMethod, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_negotiationPaymentMethods.ContainsKey(negotiationPaymentMethod.Id))
                throw new InvalidOperationException($"NegotiationPaymentMethod with id {negotiationPaymentMethod.Id} already exists");

            _negotiationPaymentMethods[negotiationPaymentMethod.Id] = negotiationPaymentMethod;

            if (!_paymentMethodsByNegotiationId.TryGetValue(negotiationPaymentMethod.NegotiationId, out var paymentMethods))
            {
                paymentMethods = new List<NegotiationPaymentMethod>();
                _paymentMethodsByNegotiationId[negotiationPaymentMethod.NegotiationId] = paymentMethods;
            }
            paymentMethods.Add(negotiationPaymentMethod);

            return Task.FromResult(negotiationPaymentMethod);
        }
    }

    public Task<NegotiationPaymentMethod> UpdateAsync(NegotiationPaymentMethod negotiationPaymentMethod, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_negotiationPaymentMethods.ContainsKey(negotiationPaymentMethod.Id))
                throw new InvalidOperationException($"NegotiationPaymentMethod with id {negotiationPaymentMethod.Id} not found");

            _negotiationPaymentMethods[negotiationPaymentMethod.Id] = negotiationPaymentMethod;
            return Task.FromResult(negotiationPaymentMethod);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var negotiationPaymentMethod = await GetByIdAsync(id, cancellationToken);
        if (negotiationPaymentMethod == null)
            return false;

        lock (_lock)
        {
            negotiationPaymentMethod.MarkAsDeleted();
            _negotiationPaymentMethods[negotiationPaymentMethod.Id] = negotiationPaymentMethod;
            return true;
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _negotiationPaymentMethods.Clear();
            _paymentMethodsByNegotiationId.Clear();
            InitializeDefaultNegotiationPaymentMethods();
        }
    }
}

