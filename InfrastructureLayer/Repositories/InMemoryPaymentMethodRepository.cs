using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryPaymentMethodRepository : IPaymentMethodRepository
{
    private readonly Dictionary<Guid, PaymentMethod> _paymentMethods = new();
    private readonly object _lock = new();

    public InMemoryPaymentMethodRepository(SeedDataService? seedDataService = null)
    {
        bool paymentMethodsLoaded = false;
        
        if (seedDataService != null)
        {
            var paymentMethods = seedDataService.GetPaymentMethods();
            foreach (var paymentMethod in paymentMethods)
            {
                try
                {
                    _paymentMethods[paymentMethod.Id] = paymentMethod;
                    paymentMethodsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default payment methods if SeedDataService is not available or no payment methods were loaded
        if (!paymentMethodsLoaded)
        {
            InitializeDefaultPaymentMethods();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var paymentMethods = seedDataService.GetPaymentMethods();
        foreach (var paymentMethod in paymentMethods)
        {
            try
            {
                _paymentMethods[paymentMethod.Id] = paymentMethod;
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultPaymentMethods()
    {
        var pm1Id = new Guid("350e8400-e29b-41d4-a716-446655440001");
        var pm1 = new PaymentMethod("Cartão de Crédito", DomainLayer.Enums.PaymentMethodType.CreditCard);
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(pm1, pm1Id);
        }
        _paymentMethods[pm1.Id] = pm1;

        var pm2Id = new Guid("350e8400-e29b-41d4-a716-446655440002");
        var pm2 = new PaymentMethod("Cartão de Débito", DomainLayer.Enums.PaymentMethodType.DebitCard);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(pm2, pm2Id);
        }
        _paymentMethods[pm2.Id] = pm2;

        var pm3Id = new Guid("350e8400-e29b-41d4-a716-446655440003");
        var pm3 = new PaymentMethod("Dinheiro", DomainLayer.Enums.PaymentMethodType.Cash);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(pm3, pm3Id);
        }
        _paymentMethods[pm3.Id] = pm3;

        var pm4Id = new Guid("350e8400-e29b-41d4-a716-446655440004");
        var pm4 = new PaymentMethod("PIX", DomainLayer.Enums.PaymentMethodType.BankTransfer);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(pm4, pm4Id);
        }
        _paymentMethods[pm4.Id] = pm4;
    }

    public Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _paymentMethods.TryGetValue(id, out var paymentMethod);
            return Task.FromResult<PaymentMethod?>(paymentMethod?.IsDeleted == false ? paymentMethod : null);
        }
    }

    public Task<IEnumerable<PaymentMethod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<PaymentMethod>>(
                _paymentMethods.Values.Where(pm => !pm.IsDeleted).ToList()
            );
        }
    }

    public Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_paymentMethods.ContainsKey(paymentMethod.Id))
                throw new InvalidOperationException($"PaymentMethod with id {paymentMethod.Id} already exists");

            _paymentMethods[paymentMethod.Id] = paymentMethod;
            return Task.FromResult(paymentMethod);
        }
    }

    public Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_paymentMethods.ContainsKey(paymentMethod.Id))
                throw new InvalidOperationException($"PaymentMethod with id {paymentMethod.Id} not found");

            _paymentMethods[paymentMethod.Id] = paymentMethod;
            return Task.FromResult(paymentMethod);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await GetByIdAsync(id, cancellationToken);
        if (paymentMethod == null)
            return false;

        lock (_lock)
        {
            paymentMethod.MarkAsDeleted();
            _paymentMethods[paymentMethod.Id] = paymentMethod;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _paymentMethods.Clear();
            
            bool paymentMethodsLoaded = false;
            if (seedDataService != null)
            {
                var paymentMethods = seedDataService.GetPaymentMethods();
                foreach (var paymentMethod in paymentMethods)
                {
                    try
                    {
                        _paymentMethods[paymentMethod.Id] = paymentMethod;
                        paymentMethodsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!paymentMethodsLoaded)
            {
                InitializeDefaultPaymentMethods();
            }
        }
    }
}

