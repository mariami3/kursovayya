using Magazin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Magazin.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReviewsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(ReviewViewModel reviewViewModel)
        {
            // Отладочная информация
            Console.WriteLine($"Received Review - Rating: {reviewViewModel.Rating}, Comment: {reviewViewModel.Comment}, BookId: {reviewViewModel.BookId}");

            if (!ModelState.IsValid)
            {
                // Логируем ошибки валидации
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        Console.WriteLine($"Error in {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                TempData["ReviewError"] = "Пожалуйста, проверьте правильность заполнения формы!";
                return RedirectToAction("DetailsBook", "Catalog", new { id = reviewViewModel.BookId });
            }

            // Проверим юзера
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["ReviewError"] = "Вы не авторизованы!";
                return RedirectToAction("DetailsBook", "Catalog", new { id = reviewViewModel.BookId });
            }

            // Создаем объект Review из ViewModel
            var review = new Review
            {
                BookId = reviewViewModel.BookId,
                UserId = userId,
                UserName = User.Identity?.Name ?? "Аноним",
                Rating = reviewViewModel.Rating,
                Comment = reviewViewModel.Comment,
                DateCreated = DateTime.Now
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/reviews", review);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {responseContent}");
                    TempData["ReviewError"] = "Ошибка при добавлении отзыва. Попробуйте позже.";
                }
                else
                {
                    TempData["ReviewSuccess"] = "Ваш отзыв успешно добавлен ✅";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                TempData["ReviewError"] = "Ошибка соединения с сервером.";
            }

            return RedirectToAction("DetailsBook", "Catalog", new { id = reviewViewModel.BookId });
        }
    }
}