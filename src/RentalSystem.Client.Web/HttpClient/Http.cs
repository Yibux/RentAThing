using RentalSystem.Shared.DTOs;
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

            else
            {
                return "Error: Account wasn't created";
            }
        }
    }
}
