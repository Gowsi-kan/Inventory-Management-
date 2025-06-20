using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagement.Models;

namespace InventoryManagement.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44315/api/Order";

        public OrderService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ResponseModel> AddOrder(Order order)
        {
            if (order is null)
                return new(false, "Order is undefined");

            try
            {
                using HttpClient client = new HttpClient();

                // Serialize the product object to JSON
                string jsonContent = JsonSerializer.Serialize(order);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send a POST request to the API
                HttpResponseMessage response = await client.PostAsync($"{BaseUrl}/AddOrderProductInventory", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                    int orderId = ((JsonElement)result["orderId"]).GetInt32();

                    Logger.WriteLogInformation($"Order successfully added with Id {orderId}");
                    return new(true, $"Order successfully added with Id {orderId}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogError($"Internal Server Error: {await response.Content.ReadAsStringAsync()}");
                    return new(false, $"Internal Server Error: {await response.Content.ReadAsStringAsync()}");
                }
                else
                {
                    Logger.WriteLogError($"Add new order failed: {await response.Content.ReadAsStringAsync()}");
                    return new(false, $"Add new order failed: {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (HttpRequestException ex)
            {
                Logger.WriteLogError($"Error adding order: {ex.Message}");
                return new(false, $"Error adding order: {ex.Message}");
            }
        }

        public async Task<ObservableCollection<Order>> LoadOrdersAsync()
        {
            try
            {
                using HttpClient client = new HttpClient();

                // Send a GET request to the API
                HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/GetAllOrders");

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var orderResponse = JsonSerializer.Deserialize<ObservableCollection<Order>>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (orderResponse != null)
                    {
                        Logger.WriteLogInformation("Orders loaded successfully.");
                        return orderResponse;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogWarning("No Orders found.");
                }
                else
                {
                    Logger.WriteLogError($"Failed to load Orders. Status code: {response.StatusCode}");
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

        public async Task<ResponseModel> UpdateOrder(Order order)
        {
            if (order is null)
                return new(false, "Order is undefined");

            try
            {
                using HttpClient client = new HttpClient();

                // Serialize the order object to JSON
                string jsonContent = JsonSerializer.Serialize(order);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send a PUT request to the API
                HttpResponseMessage response = await client.PutAsync($"{BaseUrl}/UpdateOrderProductInventory", content);

                if (response.IsSuccessStatusCode)
                {
                    Logger.WriteLogInformation($"Order with Id {order.OrderId} successfully updated");
                    return new(true, $"Order with Id {order.OrderId} successfully updated");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogError($"Order not found or internal server error: {await response.Content.ReadAsStringAsync()}");
                    return new(false, $"Order not found or internal server error: {await response.Content.ReadAsStringAsync()}");
                }
                else
                {
                    Logger.WriteLogError($"Update order failed: {await response.Content.ReadAsStringAsync()}");
                    return new(false, $"Update order failed: {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (HttpRequestException ex)
            {
                Logger.WriteLogError($"Error updating order: {ex.Message}");
                return new(false, $"Error updating order: {ex.Message}");
            }
        }

        public async Task<ResponseModel> DeleteOrder(int OrderId)
        {
            if (OrderId <= 0) return new(false, "OrderId is undefined");

            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/RemoveOrderById/{OrderId}");
                response.EnsureSuccessStatusCode();

                return new(true, "Order Deleted Successfully..");
            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error Deleting Order: {ex.Message}");
            }
        }
    }
}
