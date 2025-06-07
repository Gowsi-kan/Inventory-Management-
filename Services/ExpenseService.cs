using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagement.Models;

namespace InventoryManagement.Services
{
    public class ExpenseService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44315/api/Cashflow";

        public ExpenseService()
        {
            
        }

        public ExpenseService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<ObservableCollection<Expense>> LoadExpensesAsync()
        {
            try
            {
                using HttpClient client = new HttpClient();

                // Send a GET request to the API
                HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/GetAllCompanyExpenses");

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var expensesResponse = JsonSerializer.Deserialize<ObservableCollection<Expense>>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (expensesResponse != null)
                    {
                        Logger.WriteLogInformation("Expenses loaded successfully.");
                        return expensesResponse;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogWarning("No Expenses found.");
                }
                else
                {
                    Logger.WriteLogError($"Failed to load Expenses. Status code: {response.StatusCode}");
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

        public async Task<ResponseModel> AddExpense(Expense expense)
        {
            if (expense == null)
            {
                return new(false, "Expense is not defined.");
            }

            try
            {
                using HttpClient client = new();

                // Serialize the expense object to JSON
                string jsonContent = JsonSerializer.Serialize(expense);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send a POST request to the API
                HttpResponseMessage response = await client.PostAsync($"{BaseUrl}/AddNewCompanyExpense", content);

                if (response.IsSuccessStatusCode)
                {
                    Logger.WriteLogInformation($"Expense {expense.ExpenseName} added with ID {expense.ExpenseId}.");
                    return new(true, $"Expense {expense.ExpenseName} added with ID {expense.ExpenseId}.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogError("Internal Server Error.");
                    return new(false, "Internal Server Error.");
                }
                else
                {
                    Logger.WriteLogError($"Add new Expense failed with status code: {response.StatusCode}");
                    return new(false, $"Add new Expense failed with status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error adding new expense: {ex.Message}");
            }
        }
    }
}
