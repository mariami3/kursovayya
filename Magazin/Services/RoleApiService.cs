using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Services
{
    public class RoleApiService
    {
        private readonly HttpClient _httpClient;

        public RoleApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Role>>("api/Roles");
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Role>($"api/Roles/{id}");
        }

        public async Task CreateRoleAsync(Role role)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Roles", role);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateRoleAsync(int id, Role role)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Roles/{id}", role);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Roles/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
