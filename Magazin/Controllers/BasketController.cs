using Microsoft.AspNetCore.Mvc;
using Magazin.Models;
using System.Security.Claims;
using System.Net.Http.Json;

namespace Magazin.Controllers
{
    public class BasketController : Controller
    {
        private readonly HttpClient _httpClient;

        public BasketController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        // Просмотр корзины
        public async Task<IActionResult> Cart()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return RedirectToAction("Autorization", "Account");
                }

                int userId = int.Parse(userIdClaim);

                var cartResponse = await _httpClient.GetAsync($"/api/CartItems/User/{userId}");
                if (!cartResponse.IsSuccessStatusCode)
                {
                    return View(new List<CartItem>());
                }

                var cartItems = await cartResponse.Content.ReadFromJsonAsync<List<CartItem>>();
                return View(cartItems ?? new List<CartItem>());
            }
            catch
            {
                return View(new List<CartItem>());
            }
        }

        // Добавление книги в корзину
        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Autorization", "Account");
            }

            int userId = int.Parse(userIdClaim);

            if (quantity <= 0)
            {
                TempData["Error"] = "Количество книги не может быть меньше или равно нулю.";
                return RedirectToAction("Cart");
            }

            var requestData = new
            {
                UserId = userId,
                BookId = bookId,
                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CartItems/AddToCart", requestData);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Не удалось добавить книгу в корзину.";
            }

            return RedirectToAction("Cart"); 
        }


        // Обновление количества
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var request = new UpdateQuantityRequest { Quantity = quantity };
            var response = await _httpClient.PutAsJsonAsync($"/api/CartItems/UpdateQuantity/{cartItemId}", request);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("Cart");
        }

        // Удаление книги из корзины
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var response = await _httpClient.DeleteAsync($"/api/CartItems/RemoveFromCart/{cartItemId}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("Cart");
        }
    }

    // Модель запроса для обновления количества
    public class UpdateQuantityRequest
    {
        public int Quantity { get; set; }
    }
}
