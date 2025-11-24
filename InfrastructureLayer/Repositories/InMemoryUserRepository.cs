using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.ValueObjects;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = new();
    private readonly Dictionary<string, User> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public InMemoryUserRepository(IPasswordHasher passwordHasher, SeedDataService? seedDataService = null)
    {
        InitializeUsers(passwordHasher, seedDataService);
    }

    private void InitializeUsers(IPasswordHasher passwordHasher, SeedDataService? seedDataService)
    {
        bool usersLoaded = false;
        
        if (seedDataService != null)
        {
            var users = seedDataService.GetUsers();
            foreach (var user in users)
            {
                try
                {
                    _users[user.Id] = user;
                    _usersByEmail[user.Email.Value] = user;
                    usersLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default users if SeedDataService is not available or no users were loaded
        if (!usersLoaded)
        {
            InitializeDefaultUsers(passwordHasher);
        }
    }

    private void InitializeDefaultUsers(IPasswordHasher passwordHasher)
    {
        // User 1 - Fixed ID for consistency with seed data
        var user1Id = new Guid("850e8400-e29b-41d4-a716-446655440001");
        var user1 = new User(
            name: Name.Create("Marco Uski"),
            document: "123.456.789-00",
            birthDate: new DateTime(1990, 5, 15),
            email: Email.Create("marco.uski@huuski.com"),
            address: Address.Create(
                street: "Rua das Flores, 123",
                city: "São Paulo",
                state: "SP",
                zipCode: "01234-567",
                country: "Brasil",
                complement: "Apto 45"
            ),
            phoneNumber: PhoneNumber.Create("+55 11 98765-4321"),
            password: Password.FromHash(passwordHasher.HashPassword("Senha123!")),
            avatar: "https://m.media-amazon.com/images/M/MV5BMTk4MDM0MDUzM15BMl5BanBnXkFtZTcwOTI4MzU1Mw@@._V1_FMjpg_UX1000_.jpg"
        );

        // Set fixed ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(user1, user1Id);
        }

        _users[user1.Id] = user1;
        _usersByEmail[user1.Email.Value] = user1;

        // User 2 - Fixed ID for consistency with seed data
        var user2Id = new Guid("850e8400-e29b-41d4-a716-446655440002");
        var user2 = new User(
            name: Name.Create("Fernandinho Palmeirense"),
            document: "123.456.789-01",
            birthDate: new DateTime(1985, 8, 22),
            email: Email.Create("fernandinho.palmeirense@huuski.com"),
            address: Address.Create(
                street: "Avenida Paulista, 1000",
                city: "São Paulo",
                state: "SP",
                zipCode: "01310-100",
                country: "Brasil"
            ),
            phoneNumber: PhoneNumber.Create("+55 11 97654-3210"),
            password: Password.FromHash(passwordHasher.HashPassword("Senha456!")),
            avatar: null
        );

        // Set fixed ID using reflection
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(user2, user2Id);
        }

        _users[user2.Id] = user2;
        _usersByEmail[user2.Email.Value] = user2;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _users.TryGetValue(id, out var user);
            return Task.FromResult<User?>(user?.IsDeleted == false ? user : null);
        }
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Normalize email to lowercase for lookup (Email value object stores emails as lowercase)
            var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
            _usersByEmail.TryGetValue(normalizedEmail, out var user);
            return Task.FromResult<User?>(user?.IsDeleted == false ? user : null);
        }
    }

    public Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_usersByEmail.ContainsKey(user.Email.Value))
                throw new InvalidOperationException($"User with email {user.Email.Value} already exists");

            _users[user.Id] = user;
            _usersByEmail[user.Email.Value] = user;
            return Task.FromResult(user);
        }
    }

    public Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_users.ContainsKey(user.Id))
                throw new InvalidOperationException($"User with id {user.Id} not found");

            _users[user.Id] = user;
            _usersByEmail[user.Email.Value] = user;
            return Task.FromResult(user);
        }
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Normalize email to lowercase for lookup (Email value object stores emails as lowercase)
            var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
            var exists = _usersByEmail.TryGetValue(normalizedEmail, out var user) && user?.IsDeleted == false;
            return Task.FromResult(exists);
        }
    }

    public void Reset(IPasswordHasher passwordHasher, SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _users.Clear();
            _usersByEmail.Clear();
            InitializeUsers(passwordHasher, seedDataService);
        }
    }
}
