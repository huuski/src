using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using DomainLayer.ValueObjects;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<Guid, Customer> _customers = new();
    private readonly Dictionary<string, Customer> _customersByDocument = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Customer> _customersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Customer> _customersByPhoneNumber = new();
    private readonly object _lock = new();

    public InMemoryCustomerRepository(SeedDataService? seedDataService = null)
    {
        bool customersLoaded = false;
        
        if (seedDataService != null)
        {
            var customers = seedDataService.GetCustomers();
            foreach (var customer in customers)
            {
                try
                {
                    _customers[customer.Id] = customer;
                    _customersByDocument[customer.Document] = customer;
                    _customersByEmail[customer.Email.Value.ToLowerInvariant()] = customer;
                    var normalizedPhone = System.Text.RegularExpressions.Regex.Replace(customer.PhoneNumber.Value, @"\D", "");
                    _customersByPhoneNumber[customer.Id.ToString()] = customer;
                    customersLoaded = true;
                }
                catch
                {
                    // Skip invalid customer data
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default customers if SeedDataService is not available or no customers were loaded
        if (!customersLoaded)
        {
            InitializeDefaultCustomers();
        }
    }

    private void InitializeDefaultCustomers()
    {
        // Customer: Fernandinho Palmeirense
        var customerId = new Guid("550e8400-e29b-41d4-a716-446655440001");
        var customer = new Customer(
            Name.Create("Fernandinho Palmeirense"),
            "123.456.789-01",
            new DateTime(1985, 8, 22),
            Email.Create("fernandinho.palmeirense@huuski.com"),
            Address.Create(
                "Avenida Paulista, 1000",
                "São Paulo",
                "SP",
                "01310-100",
                "Brasil",
                null
            ),
            PhoneNumber.Create("+55 11 97654-3210")
        );
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(customer, customerId);
        }
        _customers[customer.Id] = customer;
        _customersByDocument[customer.Document] = customer;
        _customersByEmail[customer.Email.Value.ToLowerInvariant()] = customer;
        _customersByPhoneNumber[customer.Id.ToString()] = customer;
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _customers.TryGetValue(id, out var customer);
            return Task.FromResult<Customer?>(customer?.IsDeleted == false ? customer : null);
        }
    }

    public Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Normalize document by removing special characters for search
            var normalizedDocument = NormalizeDocument(document);
            
            // Try to find by normalized document
            var customer = _customersByDocument.Values
                .FirstOrDefault(c => !c.IsDeleted && 
                    NormalizeDocument(c.Document) == normalizedDocument);
            
            return Task.FromResult<Customer?>(customer);
        }
    }

    private static string NormalizeDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            return string.Empty;
        
        // Remove all non-digit characters
        return System.Text.RegularExpressions.Regex.Replace(document, @"\D", "");
    }

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Normalize email to lowercase for case-insensitive search
            var normalizedEmail = email.Trim().ToLowerInvariant();
            _customersByEmail.TryGetValue(normalizedEmail, out var customer);
            return Task.FromResult<Customer?>(customer?.IsDeleted == false ? customer : null);
        }
    }

    public Task<Customer?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Normalize phone number by removing non-digit characters for search
            var normalizedPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"\D", "");
            
            // Try to find by normalized phone
            var customer = _customersByPhoneNumber.Values
                .FirstOrDefault(c => !c.IsDeleted && 
                    System.Text.RegularExpressions.Regex.Replace(c.PhoneNumber.Value, @"\D", "") == normalizedPhone);
            
            return Task.FromResult<Customer?>(customer);
        }
    }

    public Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Customer>>(
                _customers.Values.Where(c => !c.IsDeleted).ToList()
            );
        }
    }

    public Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_customers.ContainsKey(customer.Id))
                throw new InvalidOperationException($"Customer with id {customer.Id} already exists");

            var normalizedDocument = NormalizeDocument(customer.Document);
            var existingCustomer = _customersByDocument.Values
                .FirstOrDefault(c => !c.IsDeleted && 
                    NormalizeDocument(c.Document) == normalizedDocument);
            if (existingCustomer != null)
                throw new InvalidOperationException($"Customer with document {customer.Document} already exists");

            var normalizedEmail = customer.Email.Value.ToLowerInvariant();
            if (_customersByEmail.ContainsKey(normalizedEmail))
                throw new InvalidOperationException($"Customer with email {customer.Email.Value} already exists");

            var normalizedPhone = System.Text.RegularExpressions.Regex.Replace(customer.PhoneNumber.Value, @"\D", "");
            var existingPhoneCustomer = _customersByPhoneNumber.Values
                .FirstOrDefault(c => !c.IsDeleted && 
                    System.Text.RegularExpressions.Regex.Replace(c.PhoneNumber.Value, @"\D", "") == normalizedPhone);
            if (existingPhoneCustomer != null)
                throw new InvalidOperationException($"Customer with phone number {customer.PhoneNumber.Value} already exists");

            _customers[customer.Id] = customer;
            _customersByDocument[customer.Document] = customer; // Keep original format for reference
            _customersByEmail[normalizedEmail] = customer;
            _customersByPhoneNumber[customer.Id.ToString()] = customer; // Use ID as key to avoid collisions

            return Task.FromResult(customer);
        }
    }

    public Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_customers.ContainsKey(customer.Id))
                throw new InvalidOperationException($"Customer with id {customer.Id} not found");

            var existingCustomer = _customers[customer.Id];

            // Update document index if changed (normalized)
            var existingNormalizedDoc = NormalizeDocument(existingCustomer.Document);
            var newNormalizedDoc = NormalizeDocument(customer.Document);
            if (existingNormalizedDoc != newNormalizedDoc)
            {
                _customersByDocument.Remove(existingCustomer.Document);
                _customersByDocument[customer.Document] = customer;
            }

            // Update email index if changed
            var existingEmail = existingCustomer.Email.Value.ToLowerInvariant();
            var newEmail = customer.Email.Value.ToLowerInvariant();
            if (existingEmail != newEmail)
            {
                _customersByEmail.Remove(existingEmail);
                _customersByEmail[newEmail] = customer;
            }

            // Update phone number index if changed
            var existingPhone = System.Text.RegularExpressions.Regex.Replace(existingCustomer.PhoneNumber.Value, @"\D", "");
            var newPhone = System.Text.RegularExpressions.Regex.Replace(customer.PhoneNumber.Value, @"\D", "");
            if (existingPhone != newPhone)
            {
                _customersByPhoneNumber.Remove(existingCustomer.Id.ToString());
                _customersByPhoneNumber[customer.Id.ToString()] = customer;
            }

            _customers[customer.Id] = customer;
            return Task.FromResult(customer);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await GetByIdAsync(id, cancellationToken);
        if (customer == null)
            return false;

        lock (_lock)
        {
            customer.MarkAsDeleted();
            _customers[customer.Id] = customer;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _customers.Clear();
            _customersByDocument.Clear();
            _customersByEmail.Clear();
            _customersByPhoneNumber.Clear();
            
            bool customersLoaded = false;
            if (seedDataService != null)
            {
                var customers = seedDataService.GetCustomers();
                foreach (var customer in customers)
                {
                    try
                    {
                        _customers[customer.Id] = customer;
                        _customersByDocument[customer.Document] = customer;
                        _customersByEmail[customer.Email.Value.ToLowerInvariant()] = customer;
                        var normalizedPhone = System.Text.RegularExpressions.Regex.Replace(customer.PhoneNumber.Value, @"\D", "");
                        _customersByPhoneNumber[customer.Id.ToString()] = customer;
                        customersLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!customersLoaded)
            {
                InitializeDefaultCustomers();
            }
        }
    }
}
