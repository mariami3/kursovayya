using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Services
{
    public class AdaptationApiService
    {
        private readonly HttpClient _httpClient;

        public AdaptationApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        public async Task<List<Adaptation>> GetAdaptationsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Adaptation>>("api/Adaptations");
        }

        public async Task<Adaptation?> GetAdaptationByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Adaptation>($"api/Adaptations/{id}");
        }

        public async Task CreateAdaptationAsync(Adaptation adaptation)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Adaptations", adaptation);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAdaptationAsync(int id, Adaptation adaptation)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Adaptations/{id}", adaptation);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAdaptationAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Adaptations/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
