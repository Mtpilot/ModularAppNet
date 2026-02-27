using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Barcoding.Domain.Entities;
using Modules.Barcoding.MockInfrastructure.Database;

namespace Modules.Barcoding.MockInfrastructure;

public static class MockDbSeeder
{
    public static void Seed(BarcodingDbContext context)
    {
        var invoices = new List<Invoice>();
        var checkouts = new List<InvoiceCheckout>();

        SeedInboundInvoices(invoices);
        SeedReturnInvoices(invoices);
        SeedInboundCheckouts(checkouts);
        SeedReturnCheckouts(checkouts);

        context.Invoices.AddRange(invoices);
        context.InvoiceCheckouts.AddRange(checkouts);
        context.SaveChanges();
    }

    // Workflow/step IDs — совпадают с Workflows.MockInfrastructure
    private static readonly Guid InboundId1 = Guid.Parse("0bdafc87-c7f2-439c-a454-f326d52b590c");
    private static readonly Guid InboundId2 = Guid.Parse("a78b48d0-dab9-42a0-b5fb-4fa76b8674cc");
    private static readonly Guid InboundId3 = Guid.Parse("cae4fee6-a0a3-495c-8592-793b829c0923");
    private static readonly Guid InboundW1S1 = Guid.Parse("68b71d26-9a0c-423e-b4df-beb710f217f7");
    private static readonly Guid InboundW1S2 = Guid.Parse("e8d8e01d-e526-46ea-92b9-642c0cd9dcc9");
    private static readonly Guid InboundW2S1 = Guid.Parse("c2dbe63e-7ce3-40f5-813b-c7adefa45c87");
    private static readonly Guid InboundW2S2 = Guid.Parse("ad5d407f-7ba4-4400-8558-5708a14190fe");
    private static readonly Guid InboundW3S1 = Guid.Parse("ae2ecb08-44a8-4ab4-b719-791eddf7d012");
    private static readonly Guid InboundW3S2 = Guid.Parse("8803a1fa-fa7f-4537-981d-5bbc52514eb9");

    private static readonly Guid ReturnId1 = Guid.Parse("a0dba775-8829-41dd-ba19-0bcd19627ba4");
    private static readonly Guid ReturnId2 = Guid.Parse("7dd025bc-be31-442f-942b-c6b547c1898c");
    private static readonly Guid ReturnId3 = Guid.Parse("80d5fd23-eba8-40f2-bbd6-538959f13d19");
    private static readonly Guid ReturnW1S1 = Guid.Parse("9f8faaee-bdec-4607-8c86-5a6d6fb51096");
    private static readonly Guid ReturnW1S2 = Guid.Parse("6058c88f-78ff-44bb-a733-8d94426c482a");
    private static readonly Guid ReturnW2S1 = Guid.Parse("31429522-d440-490a-9100-37386c23643d");
    private static readonly Guid ReturnW2S2 = Guid.Parse("d7228348-4112-4745-8940-8a6747526763");
    private static readonly Guid ReturnW3S1 = Guid.Parse("1b1d30a2-8752-435c-95b3-893392343733");
    private static readonly Guid ReturnW3S2 = Guid.Parse("46705528-8065-4665-a76e-90a683953636");

    private static void SeedInboundInvoices(List<Invoice> list)
    {
        var lines = SeedInboundItems();
        var baseDate = DateTime.UtcNow;

        list.Add(CreateInvoice(InboundId1, InboundW1S1, "Поставщик 1", "INV-0001", baseDate.AddDays(-1), lines));
        list.Add(CreateInvoice(InboundId2, InboundW2S1, "Поставщик 2", "INV-0002", baseDate.AddDays(-2), lines));
        list.Add(CreateInvoice(InboundId3, InboundW3S1, "Поставщик 3", "INV-0003", baseDate.AddDays(-3), lines));
    }

    private static void SeedReturnInvoices(List<Invoice> list)
    {
        var lines = SeedReturnItems();
        var baseDate = DateTime.UtcNow;

        list.Add(CreateInvoice(ReturnId1, ReturnW1S1, "Поставщик 1", "INV-0001", baseDate.AddDays(-1), lines));
        list.Add(CreateInvoice(ReturnId2, ReturnW2S1, "Поставщик 2", "INV-0002", baseDate.AddDays(-2), lines));
        list.Add(CreateInvoice(ReturnId3, ReturnW3S1, "Поставщик 3", "INV-0003", baseDate.AddDays(-3), lines));
    }

    private static Invoice CreateInvoice(Guid workflowId, Guid stepId, string counterparty, string contractNumber, DateTime date, List<SeedItem> lineItems)
    {
        return new Invoice
        {
            WorkflowId = workflowId,
            StepId = stepId,
            InvoiceNumber = Guid.NewGuid().ToString(),
            Counterparty = counterparty,
            ContractNumber = contractNumber,
            Date = date,
            Lines = lineItems.Select(l => new InvoiceLine
            {
                ProductName = l.Name,
                ConstructorName = l.Name,
                ProductCode = l.ItemCode,
                Barcode = l.ItemCode,
                Quantity = l.Quantity,
                Units = l.Units
            }).ToList()
        };
    }

    private static void SeedInboundCheckouts(List<InvoiceCheckout> list)
    {
        var baseDate = DateTime.UtcNow;
        list.Add(CreateCheckout(InboundId1, InboundW1S2, "INV-0001", "Поставщик 1", baseDate.AddDays(-1)));
        list.Add(CreateCheckout(InboundId2, InboundW2S2, "INV-0002", "Поставщик 2", baseDate.AddDays(-2)));
        list.Add(CreateCheckout(InboundId3, InboundW3S2, "INV-0003", "Поставщик 3", baseDate.AddDays(-3)));
    }

    private static void SeedReturnCheckouts(List<InvoiceCheckout> list)
    {
        var baseDate = DateTime.UtcNow;
        list.Add(CreateCheckout(ReturnId1, ReturnW1S2, "INV-0001", "Поставщик 1", baseDate.AddDays(-1)));
        list.Add(CreateCheckout(ReturnId2, ReturnW2S2, "INV-0002", "Поставщик 2", baseDate.AddDays(-2)));
        list.Add(CreateCheckout(ReturnId3, ReturnW3S2, "INV-0003", "Поставщик 3", baseDate.AddDays(-3)));
    }

    private static InvoiceCheckout CreateCheckout(Guid workflowId, Guid stepId, string contractNumber, string counterparty, DateTime date)
    {
        return new InvoiceCheckout
        {
            WorkflowId = workflowId,
            StepId = stepId,
            InvoiceNumber = Guid.NewGuid().ToString(),
            Counterparty = counterparty,
            ContractNumber = contractNumber,
            Date = date,
            TotalItems = -1,
            AcceptedItems = -1,
            MissingItems = -1,
            ExtraItems = -1
        };
    }

    private sealed record SeedItem(string Name, string ItemCode, int Quantity, string Units);

    private static List<SeedItem> SeedInboundItems() =>
    [
        new("Ноутбук Dell XPS", "LAP-DX13", 48, "шт"),
        new("Монитор 27\" 4K", "MON-4K27", 120, "шт"),
        new("Клавиатура механическая", "KEY-MK02", 200, "шт")
    ];

    private static List<SeedItem> SeedReturnItems() =>
    [
        new("Пылесос робот", "VAC-ROB2", 7, "шт"),
        new("Фен профессиональный", "DRY-PRO", 4, "шт")
    ];
}
