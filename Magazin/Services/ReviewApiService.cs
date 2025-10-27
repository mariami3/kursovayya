using System.Net.Http;
using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Services
{
    public class ReviewApiService
    {
        private readonly HttpClient _httpClient;

        public ReviewApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        public async Task<List<Review>> GetReviewsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Review>>("Reviews");
        }

        public async Task<Review?> GetReviewAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Review>($"Reviews/{id}");
        }

        public async Task<bool> CreateReviewAsync(Review review)
        {
            var response = await _httpClient.PostAsJsonAsync("Reviews", review);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateReviewAsync(int id, Review review)
        {
            var response = await _httpClient.PutAsJsonAsync($"Reviews/{id}", review);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Reviews/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
