using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using InfrastructureLayer.Data;
using InfrastructureLayer.Repositories;

namespace InfrastructureLayer.Services;

public class ResetService : IResetService
{
    private readonly IUserRepository _userRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IProductRepository _productRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IReminderRepository _reminderRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly INegotiationRepository _negotiationRepository;
    private readonly INegotiationItemRepository _negotiationItemRepository;
    private readonly INegotiationPaymentMethodRepository _negotiationPaymentMethodRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ISpotlightCardRepository _spotlightCardRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentServiceRepository _appointmentServiceRepository;
    private readonly IExecutionFlowRepository _executionFlowRepository;
    private readonly IExecutionFlowStepRepository _executionFlowStepRepository;
    private readonly IExecutionFlowStepItemRepository _executionFlowStepItemRepository;
    private readonly IExecutionFlowItemOptionRepository _executionFlowItemOptionRepository;
    private readonly ISupplyRepository _supplyRepository;
    private readonly SeedDataService _seedDataService;
    private readonly IPasswordHasher _passwordHasher;

    public ResetService(
        IUserRepository userRepository,
        IServiceRepository serviceRepository,
        IProductRepository productRepository,
        INotificationRepository notificationRepository,
        IReminderRepository reminderRepository,
        IPaymentMethodRepository paymentMethodRepository,
        INegotiationRepository negotiationRepository,
        INegotiationItemRepository negotiationItemRepository,
        INegotiationPaymentMethodRepository negotiationPaymentMethodRepository,
        ICustomerRepository customerRepository,
        ISpotlightCardRepository spotlightCardRepository,
        IVoucherRepository voucherRepository,
        IAppointmentRepository appointmentRepository,
        IAppointmentServiceRepository appointmentServiceRepository,
        IExecutionFlowRepository executionFlowRepository,
        IExecutionFlowStepRepository executionFlowStepRepository,
        IExecutionFlowStepItemRepository executionFlowStepItemRepository,
        IExecutionFlowItemOptionRepository executionFlowItemOptionRepository,
        ISupplyRepository supplyRepository,
        SeedDataService seedDataService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _reminderRepository = reminderRepository ?? throw new ArgumentNullException(nameof(reminderRepository));
        _paymentMethodRepository = paymentMethodRepository ?? throw new ArgumentNullException(nameof(paymentMethodRepository));
        _negotiationRepository = negotiationRepository ?? throw new ArgumentNullException(nameof(negotiationRepository));
        _negotiationItemRepository = negotiationItemRepository ?? throw new ArgumentNullException(nameof(negotiationItemRepository));
        _negotiationPaymentMethodRepository = negotiationPaymentMethodRepository ?? throw new ArgumentNullException(nameof(negotiationPaymentMethodRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _spotlightCardRepository = spotlightCardRepository ?? throw new ArgumentNullException(nameof(spotlightCardRepository));
        _voucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _appointmentServiceRepository = appointmentServiceRepository ?? throw new ArgumentNullException(nameof(appointmentServiceRepository));
        _executionFlowRepository = executionFlowRepository ?? throw new ArgumentNullException(nameof(executionFlowRepository));
        _executionFlowStepRepository = executionFlowStepRepository ?? throw new ArgumentNullException(nameof(executionFlowStepRepository));
        _executionFlowStepItemRepository = executionFlowStepItemRepository ?? throw new ArgumentNullException(nameof(executionFlowStepItemRepository));
        _executionFlowItemOptionRepository = executionFlowItemOptionRepository ?? throw new ArgumentNullException(nameof(executionFlowItemOptionRepository));
        _supplyRepository = supplyRepository ?? throw new ArgumentNullException(nameof(supplyRepository));
        _seedDataService = seedDataService ?? throw new ArgumentNullException(nameof(seedDataService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task ResetAllDataAsync(CancellationToken cancellationToken = default)
    {
        // Reset all repositories by calling their Reset methods
        if (_userRepository is InMemoryUserRepository userRepo)
        {
            userRepo.Reset(_passwordHasher, _seedDataService);
        }

        if (_serviceRepository is InMemoryServiceRepository serviceRepo)
        {
            serviceRepo.Reset(_seedDataService);
        }

        if (_productRepository is InMemoryProductRepository productRepo)
        {
            productRepo.Reset(_seedDataService);
        }

        if (_notificationRepository is InMemoryNotificationRepository notificationRepo)
        {
            notificationRepo.Reset(_seedDataService);
        }

        if (_reminderRepository is InMemoryReminderRepository reminderRepo)
        {
            reminderRepo.Reset(_seedDataService);
        }

        if (_paymentMethodRepository is InMemoryPaymentMethodRepository paymentMethodRepo)
        {
            paymentMethodRepo.Reset(_seedDataService);
        }

        if (_negotiationRepository is InMemoryNegotiationRepository negotiationRepo)
        {
            negotiationRepo.Reset();
        }

        if (_negotiationItemRepository is InMemoryNegotiationItemRepository negotiationItemRepo)
        {
            negotiationItemRepo.Reset();
        }

        if (_negotiationPaymentMethodRepository is InMemoryNegotiationPaymentMethodRepository negotiationPaymentMethodRepo)
        {
            negotiationPaymentMethodRepo.Reset();
        }

        if (_customerRepository is InMemoryCustomerRepository customerRepo)
        {
            customerRepo.Reset(_seedDataService);
        }

        if (_spotlightCardRepository is InMemorySpotlightCardRepository spotlightCardRepo)
        {
            spotlightCardRepo.Reset(_seedDataService);
        }

        if (_voucherRepository is InMemoryVoucherRepository voucherRepo)
        {
            voucherRepo.Reset(_seedDataService);
        }

        if (_appointmentRepository is InMemoryAppointmentRepository appointmentRepo)
        {
            appointmentRepo.Reset(_seedDataService);
        }

        if (_appointmentServiceRepository is InMemoryAppointmentServiceRepository appointmentServiceRepo)
        {
            appointmentServiceRepo.Reset(_seedDataService);
        }

        if (_executionFlowRepository is InMemoryExecutionFlowRepository executionFlowRepo)
        {
            executionFlowRepo.Reset(_seedDataService);
        }

        if (_executionFlowStepRepository is InMemoryExecutionFlowStepRepository executionFlowStepRepo)
        {
            executionFlowStepRepo.Reset(_seedDataService);
        }

        if (_executionFlowStepItemRepository is InMemoryExecutionFlowStepItemRepository executionFlowStepItemRepo)
        {
            executionFlowStepItemRepo.Reset(_seedDataService);
        }

        if (_executionFlowItemOptionRepository is InMemoryExecutionFlowItemOptionRepository executionFlowItemOptionRepo)
        {
            executionFlowItemOptionRepo.Reset(_seedDataService);
        }

        if (_supplyRepository is InMemorySupplyRepository supplyRepo)
        {
            supplyRepo.Reset(_seedDataService);
        }

        await Task.CompletedTask;
    }
}

