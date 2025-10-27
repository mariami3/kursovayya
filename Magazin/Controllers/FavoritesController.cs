using Microsoft.AspNetCore.Mvc;
using Magazin.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Magazin.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly HttpClient _httpClient;

        public FavoritesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        // Просмотр избранного
        public async Task<IActionResult> Favorite()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return RedirectToAction("Autorization", "Account");
                }

                int userId = int.Parse(userIdClaim);

                var response = await _httpClient.GetAsync($"/api/FavoriteItems/User/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    return View(new List<FavoriteItem>());
                }

                var favorites = await response.Content.ReadFromJsonAsync<List<FavoriteItem>>();
                return View(favorites ?? new List<FavoriteItem>());
            }
            catch
            {
                return View(new List<FavoriteItem>());
            }
        }

        // Добавление в избранное
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int bookId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Autorization", "Account");
            }

            int userId = int.Parse(userIdClaim);

            var requestData = new
            {
                UserId = userId,
                BookId = bookId
            };

            var response = await _httpClient.PostAsJsonAsync("/api/FavoriteItems/AddToFavorites", requestData);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Не удалось добавить книгу в избранное.";
            }

            return RedirectToAction("Favorite");
        }

        // Удаление из избранного
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int favoriteItemId)
        {
            var response = await _httpClient.DeleteAsync($"/api/FavoriteItems/RemoveFromFavorites/{favoriteItemId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Ошибка при удалении книги из избранного.";
            }

            return RedirectToAction("Favorite");
        }
    }
}
