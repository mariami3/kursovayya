using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("api/Users");
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/Users/{id}");
        }

        public async Task CreateUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Users", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUserAsync(int id, User user)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Users/{id}", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Users/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
