using Magazin.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Magazin.Services
{
    public class BookVoicesApiService
    {
        private readonly HttpClient _httpClient;

        public BookVoicesApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        // Получить все озвучки
        public async Task<List<BookVoice>> GetBookVoicesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BookVoice>>("api/BookVoices");
        }

        // Получить озвучку по ID
        public async Task<BookVoice?> GetBookVoiceByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookVoice>($"api/BookVoices/{id}");
        }

        // Получить озвучки по книге
        public async Task<List<BookVoice>> GetBookVoicesByBookAsync(int bookId)
        {
            return await _httpClient.GetFromJsonAsync<List<BookVoice>>($"api/BookVoices/ByBook/{bookId}");
        }

        // ✅ Создать новую озвучку с файлом
        public async Task CreateBookVoiceAsync(BookVoice voice, IFormFile? file)
        {
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(voice.BookId?.ToString() ?? ""), "bookId");
            form.Add(new StringContent(voice.Title), "title");
            form.Add(new StringContent(voice.DurationSeconds?.ToString() ?? ""), "durationSeconds");
            form.Add(new StringContent(voice.Format ?? ""), "format");

            if (file != null)
            {
                var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                form.Add(fileContent, "audioFile", file.FileName);
            }

            var response = await _httpClient.PostAsync("api/BookVoices", form);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateBookVoiceAsync(int id, BookVoice voice, IFormFile? audioFile)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(voice.BookId.ToString()), "bookId");
            content.Add(new StringContent(voice.Title ?? ""), "title");
            content.Add(new StringContent(voice.DurationSeconds?.ToString() ?? ""), "durationSeconds");
            content.Add(new StringContent(voice.Format ?? ""), "format");

            if (audioFile != null && audioFile.Length > 0)
            {
                var streamContent = new StreamContent(audioFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(audioFile.ContentType);
                content.Add(streamContent, "audioFile", audioFile.FileName);
            }

            var response = await _httpClient.PutAsync($"api/BookVoices/{id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBookVoiceAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/BookVoices/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}