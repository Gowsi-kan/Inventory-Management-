using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagement.Models;
using InventoryManagement.Services;
using static MudBlazor.CategoryTypes;

namespace InventoryManagement.ViewModels
{
    public partial class InventoryViewModel : ViewModelBase
    {
        private readonly ProductService _productService;

        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products 
        { 
            get => products; 
            set
            {
                products = value;
                OnPropertyChanged();
            } 
        }

        public InventoryViewModel(ProductService productService)
        {
            Products = [];

            _productService = productService;
        }


        public async Task LoadProducts()
        {
            Products = await _productService.LoadProductsAsync();
        }

        public async Task<ResponseModel> SaveProducts(Product product)
        {
            return await _productService.AddProduct(product);
        }

        public async Task<ResponseModel> UpdateProduct(Product product)
        {
            return await _productService.UpdateProduct(product);
        }

        public async Task<ResponseModel> DeleteProduct(Product product)
        {
            return await _productService.DeleteProduct(product);
        }
    }
}
