using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly Dictionary<Guid, Appointment> _appointments = new();
    private readonly Dictionary<Guid, List<Appointment>> _appointmentsByCustomerId = new();
    private readonly Dictionary<Guid, List<Appointment>> _appointmentsByUserId = new();
    private readonly object _lock = new();

    public InMemoryAppointmentRepository(SeedDataService? seedDataService = null)
    {
        bool appointmentsLoaded = false;
        
        if (seedDataService != null)
        {
            var appointments = seedDataService.GetAppointments();
            foreach (var appointment in appointments)
            {
                try
                {
                    _appointments[appointment.Id] = appointment;

                    if (!_appointmentsByCustomerId.TryGetValue(appointment.CustomerId, out var customerAppointments))
                    {
                        customerAppointments = new List<Appointment>();
                        _appointmentsByCustomerId[appointment.CustomerId] = customerAppointments;
                    }
                    customerAppointments.Add(appointment);

                    if (!_appointmentsByUserId.TryGetValue(appointment.UserId, out var userAppointments))
                    {
                        userAppointments = new List<Appointment>();
                        _appointmentsByUserId[appointment.UserId] = userAppointments;
                    }
                    userAppointments.Add(appointment);
                    appointmentsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default appointments if SeedDataService is not available or no appointments were loaded
        if (!appointmentsLoaded)
        {
            InitializeDefaultAppointments();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var appointments = seedDataService.GetAppointments();
        foreach (var appointment in appointments)
        {
            try
            {
                _appointments[appointment.Id] = appointment;

                if (!_appointmentsByCustomerId.TryGetValue(appointment.CustomerId, out var customerAppointments))
                {
                    customerAppointments = new List<Appointment>();
                    _appointmentsByCustomerId[appointment.CustomerId] = customerAppointments;
                }
                customerAppointments.Add(appointment);

                if (!_appointmentsByUserId.TryGetValue(appointment.UserId, out var userAppointments))
                {
                    userAppointments = new List<Appointment>();
                    _appointmentsByUserId[appointment.UserId] = userAppointments;
                }
                userAppointments.Add(appointment);
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultAppointments()
    {
        var customerId1 = new Guid("550e8400-e29b-41d4-a716-446655440001");
        var userId1 = new Guid("850e8400-e29b-41d4-a716-446655440001");
        
        var appointment1Id = new Guid("750e8400-e29b-41d4-a716-446655440001");
        var appointment1 = new Appointment(
            customerId1,
            userId1,
            new DateTime(2025, 12, 15, 9, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 15, 10, 0, 0, DateTimeKind.Utc),
            DomainLayer.Enums.AppointmentStatus.Scheduled
        );
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(appointment1, appointment1Id);
        }
        _appointments[appointment1.Id] = appointment1;
        AddToIndexes(appointment1);

        var appointment2Id = new Guid("750e8400-e29b-41d4-a716-446655440002");
        var appointment2 = new Appointment(
            new Guid("550e8400-e29b-41d4-a716-446655440002"),
            userId1,
            new DateTime(2025, 12, 15, 14, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 15, 15, 30, 0, DateTimeKind.Utc),
            DomainLayer.Enums.AppointmentStatus.Scheduled
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(appointment2, appointment2Id);
        }
        _appointments[appointment2.Id] = appointment2;
        AddToIndexes(appointment2);
    }

    private void AddToIndexes(Appointment appointment)
    {
        if (!_appointmentsByCustomerId.TryGetValue(appointment.CustomerId, out var customerAppointments))
        {
            customerAppointments = new List<Appointment>();
            _appointmentsByCustomerId[appointment.CustomerId] = customerAppointments;
        }
        customerAppointments.Add(appointment);

        if (!_appointmentsByUserId.TryGetValue(appointment.UserId, out var userAppointments))
        {
            userAppointments = new List<Appointment>();
            _appointmentsByUserId[appointment.UserId] = userAppointments;
        }
        userAppointments.Add(appointment);
    }

    public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _appointments.TryGetValue(id, out var appointment);
            return Task.FromResult<Appointment?>(appointment?.IsDeleted == false ? appointment : null);
        }
    }

    public Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Appointment>>(
                _appointments.Values.Where(a => !a.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<Appointment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_appointmentsByCustomerId.TryGetValue(customerId, out var appointments))
                return Task.FromResult<IEnumerable<Appointment>>(Enumerable.Empty<Appointment>());

            return Task.FromResult<IEnumerable<Appointment>>(
                appointments.Where(a => !a.IsDeleted).ToList()
            );
        }
    }

    public Task<IEnumerable<Appointment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_appointmentsByUserId.TryGetValue(userId, out var appointments))
                return Task.FromResult<IEnumerable<Appointment>>(Enumerable.Empty<Appointment>());

            return Task.FromResult<IEnumerable<Appointment>>(
                appointments.Where(a => !a.IsDeleted).ToList()
            );
        }
    }

    public Task<Appointment> CreateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_appointments.ContainsKey(appointment.Id))
                throw new InvalidOperationException($"Appointment with id {appointment.Id} already exists");

            _appointments[appointment.Id] = appointment;

            if (!_appointmentsByCustomerId.TryGetValue(appointment.CustomerId, out var customerAppointments))
            {
                customerAppointments = new List<Appointment>();
                _appointmentsByCustomerId[appointment.CustomerId] = customerAppointments;
            }
            customerAppointments.Add(appointment);

            if (!_appointmentsByUserId.TryGetValue(appointment.UserId, out var userAppointments))
            {
                userAppointments = new List<Appointment>();
                _appointmentsByUserId[appointment.UserId] = userAppointments;
            }
            userAppointments.Add(appointment);

            return Task.FromResult(appointment);
        }
    }

    public Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_appointments.ContainsKey(appointment.Id))
                throw new InvalidOperationException($"Appointment with id {appointment.Id} not found");

            _appointments[appointment.Id] = appointment;
            return Task.FromResult(appointment);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            return false;

        lock (_lock)
        {
            appointment.MarkAsDeleted();
            _appointments[appointment.Id] = appointment;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _appointments.Clear();
            _appointmentsByCustomerId.Clear();
            _appointmentsByUserId.Clear();
            
            bool appointmentsLoaded = false;
            if (seedDataService != null)
            {
                var appointments = seedDataService.GetAppointments();
                foreach (var appointment in appointments)
                {
                    try
                    {
                        _appointments[appointment.Id] = appointment;
                        AddToIndexes(appointment);
                        appointmentsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!appointmentsLoaded)
            {
                InitializeDefaultAppointments();
            }
        }
    }
}

