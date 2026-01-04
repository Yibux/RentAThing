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

        public async Task CreateUser(CreateUserRequest user, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsJsonAsync("/api/Users", user);

            //var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"STATUS: {response.StatusCode}");
            //Console.WriteLine(body);

            Console.WriteLine(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
        }
    }
}
