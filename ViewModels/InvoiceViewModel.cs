using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Services;
using QuestPDF.Fluent;

namespace InventoryManagement.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly InvoiceService _invoiceService;
        private InventoryViewModel _inventoryViewModel;

        public InvoiceViewModel(InvoiceService invoiceService, InventoryViewModel inventoryViewModel)
        {
            _invoiceService = invoiceService;
            _inventoryViewModel = inventoryViewModel;

            ProdcutIDs = [];
        }

        public List<ProductModel> Products { get; set; } = new();

        private ObservableCollection<string?> productIDs;

        public ObservableCollection<string?> ProdcutIDs
        {
            get => productIDs;
            set
            {
                productIDs = value;
                OnPropertyChanged();
            }
        }

        private string selectedProductID;

        public string SelectedProductID
        {
            get => selectedProductID;
            set
            {
                selectedProductID = value;
                OnPropertyChanged();
            }
        }

        private string productName;

        public string ProductName
        {
            get => productName;
            set
            {
                productName = value;
                OnPropertyChanged();
            }
        }

        private float quantity;

        public float Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }

        private float price;

        public float Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        private float discount = 0;

        public float Discount
        {
            get => discount;
            set
            {
                discount = value;
                OnPropertyChanged();
            }
        }

        private int payAmount = 0;

        public int PayAmount
        {
            get => payAmount;
            set
            {
                payAmount = value;
                OnPropertyChanged();
            }
        }

        public float SubTotal => Products.Sum(p => p.Quantity * p.Price);
        public float TotalDiscount => Products.Sum(p => p.Quantity * p.Discount);
        public float Total => SubTotal - TotalDiscount;
        public int Balance => (int)(PayAmount - Total);

        public void LoadProductIDs()
        {
            ProdcutIDs.Clear();
            ProdcutIDs = new ObservableCollection<string?>(_inventoryViewModel.Products.Select(p => p.ProductId));
        }

        public void AddProduct()
        {
            if (!string.IsNullOrWhiteSpace(ProductName) && Price > 0)
            {
                var newTotal = (Price - Discount) * Quantity;

                // Check if the product already exists in the list
                var existingProduct = Products.FirstOrDefault(p => p.Id == SelectedProductID);

                if (existingProduct != null)
                {
                    // Update existing product details
                    existingProduct.Name = ProductName;
                    existingProduct.Quantity = Quantity;
                    existingProduct.Price = Price;
                    existingProduct.subTotal = newTotal;
                    existingProduct.Discount = Discount;
                }
                else
                {
                    // Add new product if not found
                    Products.Add(new ProductModel
                    {
                        Id = SelectedProductID,
                        Name = ProductName,
                        Quantity = Quantity,
                        Price = Price,
                        subTotal = newTotal,
                        Discount = Discount
                    });
                }

                ClearForm();
            }
        }

        private void ClearForm()
        {
            ProductName = string.Empty;
            Price = 0;
            Quantity = 0;
            Discount = 0;
            SelectedProductID = String.Empty;
        }

        public async Task OnProductSelected(string selectedId)
        {
            if (string.IsNullOrEmpty(selectedId))
            {
                // Clear fields if no product is selected
                ProductName = "";
                Quantity = 0;
                Price = 0;
                return;
            }

            // Find the selected product
            var selectedProduct = _inventoryViewModel.Products.FirstOrDefault(p => p.ProductId == selectedId);

            if (selectedProduct != null)
            {
                SelectedProductID = selectedProduct.ProductId;
                ProductName = selectedProduct.ProductName; // Assuming InventoryVM.Products has ProductName
                Quantity = 1;  // Default quantity
                Price = selectedProduct.ProductPrice; // Assuming InventoryVM.Products has Price
            }
        }

        public async Task<string> SaveInvoiceAsPdf()
        {
            //var document = new BillingDocument(this);

            //string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "invoice.pdf");
            //await Task.Run(() => document.GeneratePdf(filePath));

            //try
            //{
            //    using var process = new System.Diagnostics.Process();
            //    process.StartInfo = new System.Diagnostics.ProcessStartInfo(filePath)
            //    {
            //        UseShellExecute = true
            //    };
            //    process.Start();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Failed to open PDF: " + ex.Message);
            //}

            var document = new BillingDocument(this);

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "invoice.pdf");
            await Task.Run(() => document.GeneratePdf(filePath));

            return filePath;
        }
    }

    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Quantity { get; set; }
        public float Price { get; set; }
        public float subTotal { get; set; }
        public float Discount { get; set; } = 0;
    }
}
