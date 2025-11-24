using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryServiceRepository : IServiceRepository
{
    private readonly Dictionary<Guid, Service> _services = new();
    private readonly object _lock = new();

    public InMemoryServiceRepository(SeedDataService? seedDataService = null)
    {
        bool servicesLoaded = false;
        
        if (seedDataService != null)
        {
            var services = seedDataService.GetServices();
            foreach (var service in services)
            {
                try
                {
                    _services[service.Id] = service;
                    servicesLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default services if SeedDataService is not available or no services were loaded
        if (!servicesLoaded)
        {
            InitializeDefaultServices();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var services = seedDataService.GetServices();
        foreach (var service in services)
        {
            try
            {
                _services[service.Id] = service;
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultServices()
    {
        var idProperty = typeof(Entity).GetProperty("Id");

        // Service 1: Limpeza de Pele
        var service1Id = new Guid("a50e8400-e29b-41d4-a716-446655440001");
        var service1 = new Service(
            "Limpeza de Pele",
            "Limpeza de pele profunda com extração de cravos e espinhas, máscara hidratante e protetor solar",
            DomainLayer.Enums.ServiceCategory.Aesthetical,
            180.00m,
            TimeSpan.FromHours(1),
            "https://images.unsplash.com/photo-1570172619644-dfd03ed5d881?w=800&h=600&fit=crop&q=80",
            new Guid("b50e8400-e29b-41d4-a716-446655440001")
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(service1, service1Id);
        }
        _services[service1.Id] = service1;

        // Service 2: Toxina Botulínica (Botox)
        var service2Id = new Guid("a50e8400-e29b-41d4-a716-446655440002");
        var service2 = new Service(
            "Toxina Botulínica (Botox)",
            "Aplicação de toxina botulínica para redução de rugas e linhas de expressão",
            DomainLayer.Enums.ServiceCategory.Aesthetical,
            450.00m,
            TimeSpan.FromMinutes(30),
            "https://images.unsplash.com/photo-1570172619644-dfd03ed5d881?w=800&h=600&fit=crop&q=80",
            new Guid("b50e8400-e29b-41d4-a716-446655440002")
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(service2, service2Id);
        }
        _services[service2.Id] = service2;

        // Service 3: Preenchimento Facial
        var service3Id = new Guid("a50e8400-e29b-41d4-a716-446655440003");
        var service3 = new Service(
            "Preenchimento Facial",
            "Preenchimento facial com ácido hialurônico para volumização e contorno facial",
            DomainLayer.Enums.ServiceCategory.Aesthetical,
            650.00m,
            TimeSpan.FromMinutes(45),
            "https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?w=800&h=600&fit=crop&q=80",
            new Guid("b50e8400-e29b-41d4-a716-446655440003")
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(service3, service3Id);
        }
        _services[service3.Id] = service3;

        // Service 4: Peeling Químico
        var service4Id = new Guid("a50e8400-e29b-41d4-a716-446655440004");
        var service4 = new Service(
            "Peeling Químico",
            "Peeling químico para renovação celular, tratamento de manchas e melhora da textura da pele",
            DomainLayer.Enums.ServiceCategory.Aesthetical,
            320.00m,
            TimeSpan.FromMinutes(60),
            "https://images.unsplash.com/photo-1612817288484-6f916006741a?w=800&h=600&fit=crop&q=80",
            new Guid("b50e8400-e29b-41d4-a716-446655440004")
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(service4, service4Id);
        }
        _services[service4.Id] = service4;

        // Service 5: Depilação a Laser
        var service5Id = new Guid("a50e8400-e29b-41d4-a716-446655440005");
        var service5 = new Service(
            "Depilação a Laser",
            "Depilação definitiva a laser em áreas selecionadas do corpo",
            DomainLayer.Enums.ServiceCategory.Aesthetical,
            280.00m,
            TimeSpan.FromMinutes(45),
            "https://images.unsplash.com/photo-1556228720-da4c4e8f4a2b?w=800&h=600&fit=crop&q=80",
            new Guid("b50e8400-e29b-41d4-a716-446655440005")
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(service5, service5Id);
        }
        _services[service5.Id] = service5;
    }

    public Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _services.TryGetValue(id, out var service);
            return Task.FromResult<Service?>(service?.IsDeleted == false ? service : null);
        }
    }

    public Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Service>>(
                _services.Values.Where(s => !s.IsDeleted).ToList()
            );
        }
    }

    public Task<Service> CreateAsync(Service service, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_services.ContainsKey(service.Id))
                throw new InvalidOperationException($"Service with id {service.Id} already exists");

            _services[service.Id] = service;
            return Task.FromResult(service);
        }
    }

    public Task<Service> UpdateAsync(Service service, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_services.ContainsKey(service.Id))
                throw new InvalidOperationException($"Service with id {service.Id} not found");

            _services[service.Id] = service;
            return Task.FromResult(service);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await GetByIdAsync(id, cancellationToken);
        if (service == null)
            return false;

        lock (_lock)
        {
            service.MarkAsDeleted();
            _services[service.Id] = service;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _services.Clear();
            
            bool servicesLoaded = false;
            if (seedDataService != null)
            {
                var services = seedDataService.GetServices();
                foreach (var service in services)
                {
                    try
                    {
                        _services[service.Id] = service;
                        servicesLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!servicesLoaded)
            {
                InitializeDefaultServices();
            }
        }
    }
}

