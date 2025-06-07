using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IBlobCache Cache;

        public UserService(IBlobCache Cache)
        {
            _httpClient = new HttpClient();
            this.Cache = Cache;
        }

        // Method to handle user login
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
                        await Cache.InsertObject("UserName", loginResponse.User.UserName);
                        await Cache.InsertObject("IsAdmin", loginResponse.User.UserPrivilege == 1);

                        var username = await Cache.GetObject<string>("UserName");
                        Logger.WriteLogInformation($"User: {loginResponse.User.UserName} Login success.");

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

    }
}
