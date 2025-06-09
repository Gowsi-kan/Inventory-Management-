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
    public class EmployeeService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44315/api/Employee";

        public EmployeeService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ObservableCollection<Employee>> LoadEmployeesAsync()
        {
            try
            {
                using HttpClient client = new HttpClient();

                // Send a GET request to the API
                HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/GetAllEmployees");

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var employeesResponse = JsonSerializer.Deserialize<ObservableCollection<Employee>>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (employeesResponse != null)
                    {
                        Logger.WriteLogInformation("Employees loaded successfully.");
                        return employeesResponse;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogWarning("No Employees found.");
                }
                else
                {
                    Logger.WriteLogError($"Failed to load Employees. Status code: {response.StatusCode}");
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

        public async Task<ResponseModel> AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                return new(false, "Employee is not defined.");
            }

            try
            {
                using HttpClient client = new();

                // Serialize the employee object to JSON
                string jsonContent = JsonSerializer.Serialize(employee);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send a POST request to the API
                HttpResponseMessage response = await client.PostAsync($"{BaseUrl}/AddNewEmployee", content);

                if (response.IsSuccessStatusCode)
                {
                    Logger.WriteLogInformation($"Employee {employee.EmployeeFirstName} added with ID {employee.EmployeeId}.");
                    return new(true, $"Employee {employee.EmployeeFirstName} added with ID {employee.EmployeeId}.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogError("Internal Server Error.");
                    return new(false, "Internal Server Error.");
                }
                else
                {
                    Logger.WriteLogError($"Add new Employee failed with status code: {response.StatusCode}");
                    return new(false, $"Add new Employee failed with status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error adding employee: {ex.Message}");
            }
        }

        public async Task<ResponseModel> UpdateEmployee(Employee employee)
        {
            if (employee == null) return new(false, "Employee is undefined");

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BaseUrl}/UpdateEmployee", content);

                response.EnsureSuccessStatusCode();

                return new(true, "Employee Updated Successfully..");
            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error updating employee: {ex.Message}");
            }
        }

        public async Task<ResponseModel> RemoveEmployee(Employee employee)
        {
            if (employee == null) return new(false, "Employee is undefined");

            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/RemoveEmployeeById/{employee.EmployeeId}");

                response.EnsureSuccessStatusCode();

                return new(true, "Employee Remopved Successfully..");
            }
            catch (HttpRequestException ex)
            {
                return new(false, $"Error removing employee: {ex.Message}");
            }
        }
    }
}
