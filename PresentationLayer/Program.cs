using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using InfrastructureLayer.Data;
using InfrastructureLayer.Repositories;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "default-issuer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "default-audience";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization - Require authentication by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Add Application Layer Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<INegotiationService, NegotiationService>();
builder.Services.AddScoped<INegotiationItemService, NegotiationItemService>();
builder.Services.AddScoped<INegotiationPaymentMethodService, NegotiationPaymentMethodService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISpotlightCardService, SpotlightCardService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IAppointmentService, ApplicationLayer.Services.AppointmentService>();
builder.Services.AddScoped<IAppointmentServiceService, AppointmentServiceService>();
builder.Services.AddScoped<IExecutionFlowService, ExecutionFlowService>();
builder.Services.AddScoped<IExecutionFlowStepService, ExecutionFlowStepService>();
builder.Services.AddScoped<IExecutionFlowStepItemService, ExecutionFlowStepItemService>();
builder.Services.AddScoped<IExecutionFlowItemOptionService, ExecutionFlowItemOptionService>();
builder.Services.AddScoped<ISupplyService, SupplyService>();

// Add Infrastructure Layer Services
builder.Services.AddSingleton<IPasswordHasher, InfrastructureLayer.Services.PasswordHasher>();
builder.Services.AddScoped<ITokenService, InfrastructureLayer.Services.TokenService>();
builder.Services.AddScoped<IPdfService, InfrastructureLayer.Services.PdfService>();
builder.Services.AddScoped<IEmailService, InfrastructureLayer.Services.EmailService>();
builder.Services.AddScoped<IResetService, InfrastructureLayer.Services.ResetService>();

// Add Seed Data Service
builder.Services.AddSingleton<SeedDataService>(sp =>
{
    var passwordHasher = sp.GetRequiredService<IPasswordHasher>();
    return new SeedDataService(passwordHasher);
});

// Add Repositories (Singleton to persist data in memory during application lifetime)
// SeedDataService is injected to load initial data from JSON
builder.Services.AddSingleton<IUserRepository>(sp => new InMemoryUserRepository(
    sp.GetRequiredService<IPasswordHasher>(),
    sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IServiceRepository>(sp => new InMemoryServiceRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IProductRepository>(sp => new InMemoryProductRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<INotificationRepository>(sp => new InMemoryNotificationRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IReminderRepository>(sp => new InMemoryReminderRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IPaymentMethodRepository>(sp => new InMemoryPaymentMethodRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<INegotiationRepository, InMemoryNegotiationRepository>();
builder.Services.AddSingleton<INegotiationItemRepository>(sp => new InMemoryNegotiationItemRepository(sp.GetRequiredService<INegotiationRepository>()));
builder.Services.AddSingleton<INegotiationPaymentMethodRepository, InMemoryNegotiationPaymentMethodRepository>();
builder.Services.AddSingleton<ICustomerRepository>(sp => new InMemoryCustomerRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<ISpotlightCardRepository>(sp => new InMemorySpotlightCardRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IVoucherRepository>(sp => new InMemoryVoucherRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IAppointmentRepository>(sp => new InMemoryAppointmentRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IAppointmentServiceRepository>(sp => new InMemoryAppointmentServiceRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IExecutionFlowRepository>(sp => new InMemoryExecutionFlowRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IExecutionFlowStepRepository>(sp => new InMemoryExecutionFlowStepRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IExecutionFlowStepItemRepository>(sp => new InMemoryExecutionFlowStepItemRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IExecutionFlowItemOptionRepository>(sp => new InMemoryExecutionFlowItemOptionRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<ISupplyRepository>(sp => new InMemorySupplyRepository(sp.GetRequiredService<SeedDataService>()));
builder.Services.AddSingleton<IRefreshTokenRepository, InMemoryRefreshTokenRepository>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Configure port from environment variable (for cloud deployments)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var portNumber))
{
    app.Urls.Add($"http://0.0.0.0:{portNumber}");
}

app.UseHttpsRedirection();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
