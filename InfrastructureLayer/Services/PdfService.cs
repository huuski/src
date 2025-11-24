using ApplicationLayer.DTOs.Customer;
using ApplicationLayer.DTOs.Negotiation;
using ApplicationLayer.DTOs.PaymentMethod;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InfrastructureLayer.Services;

public class PdfService : IPdfService
{
    private readonly INegotiationService _negotiationService;
    private readonly ICustomerService _customerService;
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PdfService(
        INegotiationService negotiationService,
        ICustomerService customerService,
        IPaymentMethodRepository paymentMethodRepository)
    {
        _negotiationService = negotiationService ?? throw new ArgumentNullException(nameof(negotiationService));
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _paymentMethodRepository = paymentMethodRepository ?? throw new ArgumentNullException(nameof(paymentMethodRepository));
    }

    public async Task<byte[]> GenerateNegotiationPdfAsync(Guid negotiationId, CancellationToken cancellationToken = default)
    {
        var negotiation = await _negotiationService.GetByIdCompleteAsync(negotiationId, cancellationToken);
        var customer = await _customerService.GetByIdAsync(negotiation.CustomerId, cancellationToken);

        // Buscar nomes dos métodos de pagamento
        var paymentMethodNames = new Dictionary<Guid, string>();
        foreach (var paymentMethodDto in negotiation.PaymentMethods)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(paymentMethodDto.PaymentMethodId, cancellationToken);
            if (paymentMethod != null)
            {
                paymentMethodNames[paymentMethodDto.PaymentMethodId] = paymentMethod.Name;
            }
        }

        QuestPDF.Settings.License = LicenseType.Community;

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Column(column =>
                            {
                                column.Item().Text("PROPOSTA COMERCIAL").FontSize(20).Bold();
                                column.Item().Text($"Código: {negotiation.Code}").FontSize(12);
                            });

                        row.ConstantItem(100)
                            .AlignRight()
                            .Text(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")).FontSize(10);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Dados do Cliente
                        column.Item().PaddingBottom(0.5f, Unit.Centimetre).Text("DADOS DO CLIENTE").FontSize(14).Bold();
                        column.Item().Background(Colors.Grey.Lighten3)
                            .Padding(10)
                            .Column(customerColumn =>
                            {
                                customerColumn.Item().Text($"Nome: {customer.Name}");
                                customerColumn.Item().Text($"Documento: {customer.Document}");
                                customerColumn.Item().Text($"Email: {customer.Email}");
                                customerColumn.Item().Text($"Telefone: {customer.PhoneNumber}");
                                customerColumn.Item().Text($"Endereço: {customer.Street}, {customer.City} - {customer.State}");
                                if (!string.IsNullOrEmpty(customer.Complement))
                                {
                                    customerColumn.Item().Text($"Complemento: {customer.Complement}");
                                }
                            });

                        column.Spacing(10);

                        // Dados da Negociação
                        column.Item().PaddingBottom(0.5f, Unit.Centimetre).Text("DADOS DA NEGOCIAÇÃO").FontSize(14).Bold();
                        column.Item().Background(Colors.Grey.Lighten3)
                            .Padding(10)
                            .Column(negotiationColumn =>
                            {
                                negotiationColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text($"Data de Expiração: {negotiation.ExpirationDate:dd/MM/yyyy}");
                                    row.RelativeItem().Text($"Valor Bruto: {negotiation.GrossValue:C}");
                                });
                                negotiationColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text($"Desconto: {negotiation.DiscountValue:C}");
                                    row.RelativeItem().Text($"Valor Líquido: {negotiation.NetValue:C}").Bold();
                                });
                            });

                        column.Spacing(10);

                        // Itens da Negociação
                        if (negotiation.Items.Any())
                        {
                            column.Item().PaddingBottom(0.5f, Unit.Centimetre).Text("ITENS DA NEGOCIAÇÃO").FontSize(14).Bold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Tipo").Bold();
                                    header.Cell().Element(CellStyle).AlignCenter().Text("Qtde").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Vlr. Unit.").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Vlr. Total").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Desconto").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Vlr. Líq.").Bold();

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Medium)
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(5);
                                    }
                                });

                                foreach (var item in negotiation.Items)
                                {
                                    table.Cell().Element(CellStyle).Text(GetItemTypeName(item.Type));
                                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.GrossValueUnit.ToString("C"));
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.GrossValue.ToString("C"));
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.DiscountValue.ToString("C"));
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.NetValue.ToString("C"));

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(5);
                                    }
                                }
                            });

                            column.Spacing(10);
                        }

                        // Métodos de Pagamento
                        if (negotiation.PaymentMethods.Any())
                        {
                            column.Item().PaddingBottom(0.5f, Unit.Centimetre).Text("MÉTODOS DE PAGAMENTO").FontSize(14).Bold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Método").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Valor").Bold();

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Medium)
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(5);
                                    }
                                });

                                foreach (var paymentMethod in negotiation.PaymentMethods)
                                {
                                    var paymentMethodName = paymentMethodNames.TryGetValue(paymentMethod.PaymentMethodId, out var name)
                                        ? name
                                        : $"Pagamento #{paymentMethod.PaymentMethodId}";
                                    table.Cell().Element(CellStyle).Text(paymentMethodName);
                                    table.Cell().Element(CellStyle).AlignRight().Text(paymentMethod.Value.ToString("C"));

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(5);
                                    }
                                }
                            });

                            column.Spacing(10);
                        }

                        // Resumo
                        column.Item().AlignRight().Background(Colors.Blue.Lighten4)
                            .Padding(10)
                            .Column(summaryColumn =>
                            {
                                summaryColumn.Item().Text("RESUMO").FontSize(12).Bold();
                                summaryColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Valor Bruto:");
                                    row.ConstantItem(120).AlignRight().Text(negotiation.GrossValue.ToString("C"));
                                });
                                summaryColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Desconto:");
                                    row.ConstantItem(120).AlignRight().Text($"-{negotiation.DiscountValue:C}");
                                });
                                summaryColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("TOTAL:").Bold();
                                    row.ConstantItem(120).AlignRight().Text(negotiation.NetValue.ToString("C"));
                                });
                            });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Documento gerado em ").FontSize(8);
                        text.Span(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")).FontSize(8).Bold();
                    });
            });
        })
        .GeneratePdf();

        return pdfBytes;
    }

    private static string GetItemTypeName(DomainLayer.Enums.NegotiationItemType type)
    {
        return type switch
        {
            DomainLayer.Enums.NegotiationItemType.Service => "Serviço",
            DomainLayer.Enums.NegotiationItemType.Package => "Pacote",
            DomainLayer.Enums.NegotiationItemType.Product => "Produto",
            _ => type.ToString()
        };
    }
}

