using System.Net.Http;
using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Services
{
    public class BookApiService
    {
        private readonly HttpClient _httpClient;

        public BookApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Book>>("api/Books");
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Book>($"api/Books/{id}");
        }

        public async Task CreateBookAsync(Book book)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Books", book);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка API: {response.StatusCode}\n{error}");
            }
        }


        public async Task UpdateBookAsync(int id, Book book)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Books/{id}", book);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBookAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Books/{id}");
            response.EnsureSuccessStatusCode();
        }


    }
}