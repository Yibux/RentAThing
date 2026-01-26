using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RentalSystem.Client.Web.RestClientNS
{
    public class RestClient
    {
        private readonly HttpClient _client;


        public RestClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> CreateUser(CreateUserRequest user, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsJsonAsync("/api/Users", user);

            Console.WriteLine($"STATUS: {response.StatusCode}");

            Console.WriteLine(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                return "Success: Account created successfully";
            }

            return "Error: Account wasn't created";

        }

        public async Task<string> UpdateUser(UpdateUserRequest user, string token, string id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync("/api/Users/" + id, user);

            Console.WriteLine($"STATUS: {response.StatusCode}");

            Console.WriteLine(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                return "Success: Account modified successfully";
            }

            return "Error: Account wasn't modified";

        }

        public async Task<UserProfile?> GetUserProfile(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Users/me");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserProfile>();
            }
            
            return null;
        }

        public async Task<string> DeleteUserProfile(string token, string id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync("/api/Users/" + id);

            if (response.IsSuccessStatusCode)
            {
                return "Success: Account deleted successfully";
            }

            return "Error: Account wasn't deleted";
        }

        public async Task<List<Item>?> GetItems(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Items");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Item>>();
            }

            return null;
        }
    }
}
