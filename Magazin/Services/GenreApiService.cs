using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Services
{
    public class GenreApiService
    {
        private readonly HttpClient _httpClient;

        public GenreApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Genre>>("api/Genres");
        }

        public async Task<Genre?> GetGenreByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Genre>($"api/Genres/{id}");
        }

        public async Task CreateGenreAsync(Genre genre)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Genres", genre);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateGenreAsync(int id, Genre genre)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Genres/{id}", genre);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteGenreAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Genres/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
