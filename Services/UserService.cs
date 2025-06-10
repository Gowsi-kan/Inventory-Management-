using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Akavache;
using InventoryManagement.Models;
using InventoryManagement.ViewModels;
using static InventoryManagement.Components.LoginComponent;

namespace InventoryManagement.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44315/api/User";
        private readonly IBlobCache Cache;

        public UserService(IBlobCache Cache)
        {
            _httpClient = new HttpClient();
            this.Cache = Cache;
        }

        public async Task<bool> LoginUser(LoginModel model)
        {
            // API endpoint for login
            string apiUrl = "https://localhost:44315/api/User/GetUserByCredential";

            // User credentials to send in the request body
            var credentials = new
            {
                UserName = model.Username,
                UserPassword = model.Password,
            };

            try
            {
                using HttpClient client = new HttpClient();
                // Serialize the credentials object to JSON
                string jsonCredentials = JsonSerializer.Serialize(credentials);
                var content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");

                // Send a POST request to the API
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (loginResponse != null)
                    {
                        await Cache.InsertObject("IsAuthenticated", true);
                        await Cache.InsertObject("UserName", loginResponse.User.userName);
                        await Cache.InsertObject("IsAdmin", loginResponse.User.userPrivilege == 1);

                        var username = await Cache.GetObject<string>("UserName");
                        Logger.WriteLogInformation($"User: {loginResponse.User.userName} Login success.");

                        return true;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogError("Incorrect username or password.");
                }
                else
                {
                    Logger.WriteLogError($"Login failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLogError($"Exception occurred during login: {ex.Message}");
            }
            return false;
        }

        public async Task<ObservableCollection<User>> LoadUsersAsync()
        {
            try
            {
                using HttpClient client = new HttpClient();

                // Send a GET request to the API
                HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/GetAllUsers");

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var userResponse = JsonSerializer.Deserialize<ObservableCollection<User>>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (userResponse != null)
                    {
                        Logger.WriteLogInformation("Users loaded successfully.");
                        return userResponse;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.WriteLogWarning("No Users found.");
                }
                else
                {
                    Logger.WriteLogError($"Failed to load Users. Status code: {response.StatusCode}");
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

        public async Task<ResponseModel> AddUser(User user)
        {
            if (user == null)
            {
                return new(false, "User is not defined.");
            }

            try
            {
                using HttpClient client = new HttpClient();

                // Serialize the user object to JSON
                string jsonUser = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

                // Send POST request to the API
                HttpResponseMessage response = await client.PostAsync($"{BaseUrl}/AddUser", content);

                // Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Try to deserialize into a standard response model
                var apiResponse = JsonSerializer.Deserialize<Response>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                {
                    Logger.WriteLogInformation($"User: {user.userName} registered successfully.");
                    return new(true, $"User: {user.userName} registered successfully.");
                }
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    Logger.WriteLogWarning($"Username '{user.userName}' already exists. Message: {apiResponse?.Message}");
                    return new(false, $"Username '{user.userName}' already exists. Message: {apiResponse?.Message}");
                }
                else
                {
                    Logger.WriteLogError($"User registration failed. Status: {response.StatusCode}, Message: {apiResponse?.Message ?? "No details"}");
                    return new(false, $"User registration failed. Status: {response.StatusCode}, Message: {apiResponse?.Message ?? "No details"}");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLogError($"Exception occurred during user registration: {ex.Message}");
                return new(false, $"Exception occurred during user registration: {ex.Message}");
            }
        }

    }
}
