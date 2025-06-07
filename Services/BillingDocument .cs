using System.Collections.Generic;
using InventoryManagement.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace InventoryManagement.Services
{
    public class BillingDocument : IDocument
    {
        private readonly InvoiceViewModel invoice;
        private readonly string invoiceNumber = "INV-" + new Random().Next(1000, 9999);
        private readonly DateTime now = DateTime.Now;

        public BillingDocument(InvoiceViewModel invoice)
        {
            this.invoice = invoice;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        [Obsolete]
        public void Compose(IDocumentContainer container)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Row(row => {
                    row.Spacing(10);

                    row.RelativeColumn().Column(col =>
                    {
                        col.Item().Text("INVOICE").FontSize(24).SemiBold().FontColor(Colors.Black);
                    });

                    row.ConstantColumn(100).AlignRight().Height(40).Image(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Icon_App.ico"), ImageScaling.FitArea); // logo
                });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Row(row =>
                    {
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("Bill To").Bold();
                            col.Item().Text("Jessie M Horne\n4312 Wood Road\nNew York, NY 10031");
                        });

                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("Ship To").Bold();
                            col.Item().Text("Jessie M Horne\n2019 Redbud Drive\nNew York, NY 10011");
                        });

                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text($"Invoice #: {invoiceNumber}");
                            col.Item().Text($"Invoice Date: {now:dd/MM/yyyy}");
                            col.Item().Text($"P.O.#: 2412/2019");
                            col.Item().Text($"Due Date: {now.AddDays(15):dd/MM/yyyy}");
                        });
                    });

                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);  // Qty
                            cols.RelativeColumn();    // Description
                            cols.ConstantColumn(80);  // Unit Price
                            cols.ConstantColumn(80);  // Amount
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Qty");
                            header.Cell().Element(CellStyle).Text("Description");
                            header.Cell().Element(CellStyle).AlignRight().Text("Unit Price");
                            header.Cell().Element(CellStyle).AlignRight().Text("Amount");

                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold())
                                         .Background(Colors.Grey.Lighten3)
                                         .PaddingVertical(5)
                                         .PaddingHorizontal(3);
                        });

                        foreach (var item in invoice.Products)
                        {
                            table.Cell().Padding(5).Text(item.Quantity.ToString());
                            table.Cell().Padding(5).Text(item.Name);
                            table.Cell().Padding(5).AlignRight().Text($"{item.Price:0.00}");
                            table.Cell().Padding(5).AlignRight().Text($"{item.subTotal:0.00}");
                        }
                    });

                    column.Item().AlignRight().Column(summary =>
                    {
                        summary.Spacing(3);
                        summary.Item().Row(row =>
                        {
                            row.RelativeColumn().Text("Subtotal:");
                            row.ConstantColumn(80).AlignRight().Text($"Rs {invoice.SubTotal:0.00}");
                        });
                        summary.Item().Row(row =>
                        {
                            row.RelativeColumn().Text("Discount:");
                            row.ConstantColumn(80).AlignRight().Text($"Rs {invoice.TotalDiscount:0.00}");
                        });
                        summary.Item().Row(row =>
                        {
                            row.RelativeColumn().Text("Total:").Bold();
                            row.ConstantColumn(80).AlignRight().Text($"Rs {invoice.Total:0.00}").Bold();
                        });
                        summary.Item().Row(row =>
                        {
                            row.RelativeColumn().Text("Paid:");
                            row.ConstantColumn(80).AlignRight().Text($"Rs {invoice.PayAmount:0.00}");
                        });
                        summary.Item().Row(row =>
                        {
                            row.RelativeColumn().Text("Balance:");
                            row.ConstantColumn(80).AlignRight().Text($"Rs {invoice.Balance:0.00}");
                        });
                    });
                });

                page.Footer().Column(col =>
                {
                    col.Spacing(4);
                    col.Item().Text("Terms & Conditions").Bold();
                    col.Item().Text("Payment is due within 15 days");
                    col.Item().Text("Name of Bank\nAccount number: 1234567890\nRouting: 098765432");
                });
            });
        }
    }

    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal subTotal { get; set; }
    }
}
