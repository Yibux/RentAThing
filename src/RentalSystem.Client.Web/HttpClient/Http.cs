using Newtonsoft.Json.Linq;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RentalSystem.Client.Web.Http
{
    public class UsersApiClient
    {
        private readonly HttpClient _client;


        public UsersApiClient(HttpClient client)
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
    }
}
