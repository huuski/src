using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryAppointmentServiceRepository : IAppointmentServiceRepository
{
    private readonly Dictionary<Guid, AppointmentService> _appointmentServices = new();
    private readonly Dictionary<Guid, List<AppointmentService>> _appointmentServicesByAppointmentId = new();
    private readonly object _lock = new();

    public InMemoryAppointmentServiceRepository(SeedDataService? seedDataService = null)
    {
        bool appointmentServicesLoaded = false;
        
        if (seedDataService != null)
        {
            var appointmentServices = seedDataService.GetAppointmentServices();
            foreach (var appointmentService in appointmentServices)
            {
                try
                {
                    _appointmentServices[appointmentService.Id] = appointmentService;

                    if (!_appointmentServicesByAppointmentId.TryGetValue(appointmentService.AppointmentId, out var appointmentServicesList))
                    {
                        appointmentServicesList = new List<AppointmentService>();
                        _appointmentServicesByAppointmentId[appointmentService.AppointmentId] = appointmentServicesList;
                    }
                    appointmentServicesList.Add(appointmentService);
                    appointmentServicesLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default appointment services if SeedDataService is not available or no appointment services were loaded
        if (!appointmentServicesLoaded)
        {
            InitializeDefaultAppointmentServices();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var appointmentServices = seedDataService.GetAppointmentServices();
        foreach (var appointmentService in appointmentServices)
        {
            try
            {
                _appointmentServices[appointmentService.Id] = appointmentService;

                if (!_appointmentServicesByAppointmentId.TryGetValue(appointmentService.AppointmentId, out var appointmentServicesList))
                {
                    appointmentServicesList = new List<AppointmentService>();
                    _appointmentServicesByAppointmentId[appointmentService.AppointmentId] = appointmentServicesList;
                }
                appointmentServicesList.Add(appointmentService);
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultAppointmentServices()
    {
        // Create appointment services for the default appointments
        var appointmentId1 = new Guid("750e8400-e29b-41d4-a716-446655440001");
        var appointmentId2 = new Guid("750e8400-e29b-41d4-a716-446655440002");
        var serviceId1 = new Guid("a50e8400-e29b-41d4-a716-446655440001");
        var serviceId2 = new Guid("a50e8400-e29b-41d4-a716-446655440002");
        
        // AppointmentService 1 - for appointment 1
        var appointmentService1 = new AppointmentService(
            appointmentId1,
            serviceId1,
            1,
            5,
            DomainLayer.Enums.AppointmentServiceStatus.Pending,
            "Primeira sessão de consulta médica. Paciente apresentou sintomas leves."
        );
        _appointmentServices[appointmentService1.Id] = appointmentService1;
        AddToIndex(appointmentService1);

        // AppointmentService 2 - for appointment 1
        var appointmentService2 = new AppointmentService(
            appointmentId1,
            serviceId2,
            2,
            5,
            DomainLayer.Enums.AppointmentServiceStatus.Pending,
            null
        );
        _appointmentServices[appointmentService2.Id] = appointmentService2;
        AddToIndex(appointmentService2);

        // AppointmentService 3 - for appointment 2
        var appointmentService3 = new AppointmentService(
            appointmentId2,
            serviceId1,
            1,
            3,
            DomainLayer.Enums.AppointmentServiceStatus.InProgress,
            "Consulta em andamento"
        );
        _appointmentServices[appointmentService3.Id] = appointmentService3;
        AddToIndex(appointmentService3);
    }

    private void AddToIndex(AppointmentService appointmentService)
    {
        if (!_appointmentServicesByAppointmentId.TryGetValue(appointmentService.AppointmentId, out var appointmentServicesList))
        {
            appointmentServicesList = new List<AppointmentService>();
            _appointmentServicesByAppointmentId[appointmentService.AppointmentId] = appointmentServicesList;
        }
        appointmentServicesList.Add(appointmentService);
    }

    public Task<AppointmentService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _appointmentServices.TryGetValue(id, out var appointmentService);
            return Task.FromResult<AppointmentService?>(appointmentService?.IsDeleted == false ? appointmentService : null);
        }
    }

    public Task<IEnumerable<AppointmentService>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<AppointmentService>>(
                _appointmentServices.Values.Where(a => !a.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<AppointmentService>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_appointmentServicesByAppointmentId.TryGetValue(appointmentId, out var appointmentServices))
                return Task.FromResult<IEnumerable<AppointmentService>>(Enumerable.Empty<AppointmentService>());

            return Task.FromResult<IEnumerable<AppointmentService>>(
                appointmentServices.Where(a => !a.IsDeleted).ToList()
            );
        }
    }

    public Task<AppointmentService> CreateAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_appointmentServices.ContainsKey(appointmentService.Id))
                throw new InvalidOperationException($"AppointmentService with id {appointmentService.Id} already exists");

            _appointmentServices[appointmentService.Id] = appointmentService;

            if (!_appointmentServicesByAppointmentId.TryGetValue(appointmentService.AppointmentId, out var appointmentServices))
            {
                appointmentServices = new List<AppointmentService>();
                _appointmentServicesByAppointmentId[appointmentService.AppointmentId] = appointmentServices;
            }
            appointmentServices.Add(appointmentService);

            return Task.FromResult(appointmentService);
        }
    }

    public Task<AppointmentService> UpdateAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_appointmentServices.ContainsKey(appointmentService.Id))
                throw new InvalidOperationException($"AppointmentService with id {appointmentService.Id} not found");

            _appointmentServices[appointmentService.Id] = appointmentService;
            return Task.FromResult(appointmentService);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointmentService = await GetByIdAsync(id, cancellationToken);
        if (appointmentService == null)
            return false;

        lock (_lock)
        {
            appointmentService.MarkAsDeleted();
            _appointmentServices[appointmentService.Id] = appointmentService;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _appointmentServices.Clear();
            _appointmentServicesByAppointmentId.Clear();
            
            bool appointmentServicesLoaded = false;
            if (seedDataService != null)
            {
                var appointmentServices = seedDataService.GetAppointmentServices();
                foreach (var appointmentService in appointmentServices)
                {
                    try
                    {
                        _appointmentServices[appointmentService.Id] = appointmentService;
                        AddToIndex(appointmentService);
                        appointmentServicesLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!appointmentServicesLoaded)
            {
                InitializeDefaultAppointmentServices();
            }
        }
    }
}

