using Magazin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Magazin.Controllers
{
    public class CatalogController : Controller
    {
        private readonly HttpClient _httpClient;

        public CatalogController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        //  Каталог книг
        public async Task<IActionResult> Catalog(int? genreId, string searchTerm, string sortOrder)
        {
            // Загружаем жанры
            var genres = await _httpClient.GetFromJsonAsync<List<Genre>>("/api/genres");

            ViewBag.GenreOptions = genres?
                .Select(g => new SelectListItem
                {
                    Value = g.IdGenre.ToString(),
                    Text = g.NameGenre,
                    Selected = g.IdGenre == genreId
                })
                .ToList();

            ViewBag.SelectedGenreId = genreId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;

            // Загружаем книги
            var books = await _httpClient.GetFromJsonAsync<List<Book>>("/api/books");

            if (books == null)
            {
                books = new List<Book>();
            }

            // Фильтрация по жанру
            if (genreId.HasValue)
            {
                books = books.Where(b => b.GenreId == genreId.Value).ToList();
            }

            // Поиск по названию
            if (!string.IsNullOrEmpty(searchTerm))
            {
                books = books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Сортировка
            books = sortOrder switch
            {
                "PriceAsc" => books.OrderBy(b => b.Price).ToList(),
                "PriceDesc" => books.OrderByDescending(b => b.Price).ToList(),
                _ => books
            };

            return View(books);
        }

        //  Детали книги
        [HttpGet]
        public async Task<IActionResult> DetailsBook(int id)
        {
            // Загружаем книгу
            var bookResponse = await _httpClient.GetAsync($"/api/books/{id}");
            if (!bookResponse.IsSuccessStatusCode)
                return NotFound();

            var book = await bookResponse.Content.ReadFromJsonAsync<Book>();
            if (book == null)
                return NotFound();

            // Загружаем отзывы
            List<Review> reviews = new();
            var reviewsResponse = await _httpClient.GetAsync($"/api/reviews/book/{id}");
            if (reviewsResponse.IsSuccessStatusCode)
            {
                var loadedReviews = await reviewsResponse.Content.ReadFromJsonAsync<List<Review>>();
                if (loadedReviews != null)
                    reviews = loadedReviews;
            }

            // Загружаем озвучки (BookVoice)
            List<BookVoice> voices = new();
            var voiceResponse = await _httpClient.GetAsync($"/api/BookVoices/ByBook/{id}");
            if (voiceResponse.IsSuccessStatusCode)
            {
                var loadedVoices = await voiceResponse.Content.ReadFromJsonAsync<List<BookVoice>>();
                if (loadedVoices != null)
                    voices = loadedVoices;
            }

            ViewBag.Reviews = reviews;
            ViewBag.BookVoices = voices;

            return View(book);
        }


    }
} 