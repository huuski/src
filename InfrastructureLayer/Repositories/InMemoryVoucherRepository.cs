using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryVoucherRepository : IVoucherRepository
{
    private readonly Dictionary<Guid, Voucher> _vouchers = new();
    private readonly Dictionary<string, Voucher> _vouchersByCode = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public InMemoryVoucherRepository(SeedDataService? seedDataService = null)
    {
        bool vouchersLoaded = false;
        
        if (seedDataService != null)
        {
            var vouchers = seedDataService.GetVouchers();
            foreach (var voucher in vouchers)
            {
                try
                {
                    _vouchers[voucher.Id] = voucher;
                    var normalizedCode = voucher.Code.Trim().ToUpperInvariant();
                    _vouchersByCode[normalizedCode] = voucher;
                    vouchersLoaded = true;
                }
                catch
                {
                    // Skip invalid voucher data
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default vouchers if SeedDataService is not available or no vouchers were loaded
        if (!vouchersLoaded)
        {
            InitializeDefaultVouchers();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var vouchers = seedDataService.GetVouchers();
        foreach (var voucher in vouchers)
        {
            try
            {
                _vouchers[voucher.Id] = voucher;
                var normalizedCode = voucher.Code.Trim().ToUpperInvariant();
                _vouchersByCode[normalizedCode] = voucher;
            }
            catch
            {
                // Skip invalid voucher data
                continue;
            }
        }
    }

    private void InitializeDefaultVouchers()
    {
        var voucher1 = new Voucher(
            "DESCONTO10",
            "Cupom de desconto de R$ 10,00 em qualquer compra acima de R$ 50,00",
            10.00m,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            true,
            50.00m
        );
        _vouchers[voucher1.Id] = voucher1;
        _vouchersByCode[voucher1.Code.ToUpperInvariant()] = voucher1;

        var voucher2 = new Voucher(
            "BEMVINDO20",
            "Desconto de R$ 20,00 para novos clientes. Válido na primeira compra.",
            20.00m,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 6, 30),
            true,
            100.00m
        );
        _vouchers[voucher2.Id] = voucher2;
        _vouchersByCode[voucher2.Code.ToUpperInvariant()] = voucher2;

        var voucher3 = new Voucher(
            "PROMOVERÃO",
            "Cupom especial de verão - desconto de R$ 30,00 em compras acima de R$ 150,00",
            30.00m,
            new DateTime(2025, 12, 1),
            new DateTime(2026, 3, 31),
            true,
            150.00m
        );
        _vouchers[voucher3.Id] = voucher3;
        _vouchersByCode[voucher3.Code.ToUpperInvariant()] = voucher3;
    }

    public Task<Voucher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _vouchers.TryGetValue(id, out var voucher);
            return Task.FromResult<Voucher?>(voucher?.IsDeleted == false ? voucher : null);
        }
    }

    public Task<Voucher?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Normalize code to lowercase for case-insensitive search
            var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;
            _vouchersByCode.TryGetValue(normalizedCode, out var voucher);
            return Task.FromResult<Voucher?>(voucher?.IsDeleted == false ? voucher : null);
        }
    }

    public Task<IEnumerable<Voucher>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Voucher>>(
                _vouchers.Values.Where(v => !v.IsDeleted).ToList()
            );
        }
    }

    public Task<Voucher> CreateAsync(Voucher voucher, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_vouchers.ContainsKey(voucher.Id))
                throw new InvalidOperationException($"Voucher with id {voucher.Id} already exists");

            var normalizedCode = voucher.Code.Trim().ToUpperInvariant();
            if (_vouchersByCode.ContainsKey(normalizedCode))
                throw new InvalidOperationException($"Voucher with code {voucher.Code} already exists");

            _vouchers[voucher.Id] = voucher;
            _vouchersByCode[normalizedCode] = voucher;

            return Task.FromResult(voucher);
        }
    }

    public Task<Voucher> UpdateAsync(Voucher voucher, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_vouchers.ContainsKey(voucher.Id))
                throw new InvalidOperationException($"Voucher with id {voucher.Id} not found");

            var existingVoucher = _vouchers[voucher.Id];

            // Update code index if changed
            var existingNormalizedCode = existingVoucher.Code.Trim().ToUpperInvariant();
            var newNormalizedCode = voucher.Code.Trim().ToUpperInvariant();
            if (existingNormalizedCode != newNormalizedCode)
            {
                // Check if new code already exists and belongs to another non-deleted voucher
                if (_vouchersByCode.TryGetValue(newNormalizedCode, out var existingVoucherWithCode) && 
                    existingVoucherWithCode.Id != voucher.Id && 
                    !existingVoucherWithCode.IsDeleted)
                    throw new InvalidOperationException($"Voucher with code {voucher.Code} already exists");

                _vouchersByCode.Remove(existingNormalizedCode);
                _vouchersByCode[newNormalizedCode] = voucher;
            }

            _vouchers[voucher.Id] = voucher;
            return Task.FromResult(voucher);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var voucher = await GetByIdAsync(id, cancellationToken);
        if (voucher == null)
            return false;

        lock (_lock)
        {
            voucher.MarkAsDeleted();
            _vouchers[voucher.Id] = voucher;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _vouchers.Clear();
            _vouchersByCode.Clear();
            
            bool vouchersLoaded = false;
            if (seedDataService != null)
            {
                var vouchers = seedDataService.GetVouchers();
                foreach (var voucher in vouchers)
                {
                    try
                    {
                        _vouchers[voucher.Id] = voucher;
                        _vouchersByCode[voucher.Code.Trim().ToUpperInvariant()] = voucher;
                        vouchersLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!vouchersLoaded)
            {
                InitializeDefaultVouchers();
            }
        }
    }
}

