using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Services
{
    public class AuthorApiService
    {
        private readonly HttpClient _httpClient;

        public AuthorApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        public async Task<List<Author>> GetAuthorsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Author>>("api/Authors");
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Author>($"api/Authors/{id}");
        }

        public async Task CreateAuthorAsync(Author author)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Authors", author);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAuthorAsync(int id, Author author)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Authors/{id}", author);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Authors/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
