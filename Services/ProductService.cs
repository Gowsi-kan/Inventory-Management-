using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagement.Models;
using static MudBlazor.CategoryTypes;

namespace InventoryManagement.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44315/api/Product";

        public ProductService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ObservableCollection<Product>> LoadProductsAsync()
        {
            try
            {
                using HttpClient client = new HttpClient();

                // Send a GET request to the API
                HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/GetAllProducts");

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var productsResponse = JsonSerializer.Deserialize<ObservableCollection<Product>>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (productsResponse != null)
                    {
                        Logger.WriteLogInformation("Products loaded successfully.");
                        return productsResponse;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogWarning("No products found.");
                }
                else
                {
                    Logger.WriteLogError($"Failed to load products. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Logger.WriteLogError($"HttpRequestException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.WriteLogError($"Unexpected error: {ex.Message}");
            }

            return [];
        }

        public async Task<ResponseModel> AddProduct(Product product)
        {
            if (product == null)
                return new(false, "Product is undefined");

            try
            {
                // Step 1: Get the latest Product ID
                string latestProductId = await GetLatestProductId();
                string newProductId = GenerateNextProductId(latestProductId);
                product.ProductId = newProductId; // Assign the new Product ID

                using HttpClient client = new HttpClient();

                // Serialize the product object to JSON
                string jsonContent = JsonSerializer.Serialize(product);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send a POST request to the API
                HttpResponseMessage response = await client.PostAsync($"{BaseUrl}/CreateProduct", content);

                if (response.IsSuccessStatusCode)
                {
                    Logger.WriteLogInformation($"Product {product.ProductName} added with ID {newProductId}.");
                    return new(true, $"Product {product.ProductName} added with ID {newProductId}.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogError("Internal Server Error.");
                    return new(false, "Internal Server Error.");
                }
                else
                {
                    Logger.WriteLogError($"Add new product failed with status code: {response.StatusCode}");
                    return new(false, $"Add new product failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error adding product: {ex.Message}");
            }
        }

        // Method to get the latest Product ID from the API
        private async Task<string> GetLatestProductId()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/GetLatestProductId");

                if (response.IsSuccessStatusCode)
                {
                    string latestProductId = await response.Content.ReadAsStringAsync();
                    return latestProductId.Trim(); // Ensure no extra spaces
                }
                else
                {
                    Logger.WriteLogError("Failed to retrieve latest Product ID.");
                    return "P300"; // Default value if no products exist
                }
            }
        }

        // Method to generate the next Product ID
        private string GenerateNextProductId(string latestProductId)
        {
            if (string.IsNullOrEmpty(latestProductId) || !latestProductId.StartsWith("P"))
                return "P300"; // Start from P300 if no valid ID is found

            string numberPart = latestProductId.Substring(1); // Extract numeric part
            if (int.TryParse(numberPart, out int numericId))
            {
                return $"P{numericId + 1}"; // Increment and return new Product ID
            }

            return "P300"; // Fallback
        }

        public async Task<ResponseModel> UpdateProduct(Product SelectedProduct)
        {
            if (SelectedProduct == null) return new(false, "Product is undefined");

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(SelectedProduct), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BaseUrl}/UpdateProduct", content);
                response.EnsureSuccessStatusCode();

                return new(true, "Product Update Successfully..");
            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error updating product: {ex.Message}");
            }
        }

        public async Task<ResponseModel> DeleteProduct(Product SelectedProduct)
        {
            if (SelectedProduct == null) return new(false, "Product is undefined");

            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/RemoveProductById/{SelectedProduct.ProductId}");
                response.EnsureSuccessStatusCode();

                return new(true, "Product Deleted Successfully..");
            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error Deleting Product: {ex.Message}");
            }
        }
    }
}
