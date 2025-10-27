using Magazin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Magazin.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                // Проверяем авторизацию
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return RedirectToAction("Autorization", "Account");

                int userId = int.Parse(userIdClaim);

                // Получаем пользователя
                var userResponse = await _httpClient.GetAsync($"/api/users/{userId}");
                if (!userResponse.IsSuccessStatusCode)
                    return RedirectToAction("Autorization", "Account");

                var user = await userResponse.Content.ReadFromJsonAsync<User>();

                // Получаем корзину (список позиций)
                var basketResponse = await _httpClient.GetAsync($"/api/CartItems/User/{userId}");
                if (!basketResponse.IsSuccessStatusCode)
                    return RedirectToAction("Cart", "Basket");

                var basket = await basketResponse.Content.ReadFromJsonAsync<List<CartItem>>();

                if (basket == null || !basket.Any())
                {
                    TempData["ErrorMessage"] = "Корзина пуста!";
                    return RedirectToAction("Cart", "Basket");
                }

                // Формируем заказ
                var order = new Order
                {
                    UserId = user.IdUser,
                    Date = DateTime.Now,
                    StatusOrders = "В обработке",
                    TotalSum = basket.Sum(i => i.Quantity * i.Book.Price),
                    OrderItems = basket.Select(i => new OrderItem
                    {
                        BookId = i.Book.IdBook,
                        Quantity = i.Quantity,
                        Price = i.Book.Price
                    }).ToList()
                };

                // Отправляем заказ в API
                var orderResponse = await _httpClient.PostAsJsonAsync("/api/orders", order);
                if (!orderResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при оформлении заказа.";
                    return RedirectToAction("Cart", "Basket");
                }

                // Очищаем корзину
                await _httpClient.DeleteAsync($"/api/basket/clear/{userId}");

                TempData["SuccessMessage"] = "Ваш заказ успешно оформлен!";
                return RedirectToAction("List", "Order");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("Cart", "Basket");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutSingle(int cartItemId)
        {
            try
            {
                // Проверяем авторизацию
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return RedirectToAction("Autorization", "Account");

                int userId = int.Parse(userIdClaim);

                // Получаем корзину пользователя
                var basketResponse = await _httpClient.GetAsync($"/api/CartItems/User/{userId}");
                if (!basketResponse.IsSuccessStatusCode)
                    return RedirectToAction("Cart", "Basket");

                var basket = await basketResponse.Content.ReadFromJsonAsync<List<CartItem>>();
                if (basket == null || !basket.Any())
                {
                    TempData["ErrorMessage"] = "Корзина пуста!";
                    return RedirectToAction("Cart", "Basket");
                }

                // Находим нужный товар
                var cartItem = basket.FirstOrDefault(i => i.IdCartItem == cartItemId);
                if (cartItem == null)
                {
                    TempData["ErrorMessage"] = "Товар не найден в корзине!";
                    return RedirectToAction("Cart", "Basket");
                }

                // Формируем заказ только для этого товара
                var order = new Order
                {
                    UserId = userId,
                    Date = DateTime.Now,
                    StatusOrders = "В обработке",
                    TotalSum = cartItem.Quantity * cartItem.Book.Price,
                    OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    BookId = cartItem.Book.IdBook,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Book.Price
                }
            }
                };

                // Отправляем заказ в API
                var orderResponse = await _httpClient.PostAsJsonAsync("/api/orders", order);
                if (!orderResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при оформлении заказа.";
                    return RedirectToAction("Cart", "Basket");
                }

                // Удаляем оформленный товар из корзины
                await _httpClient.DeleteAsync($"/api/CartItems/RemoveFromCart/{cartItemId}");

                TempData["SuccessMessage"] = "Товар успешно оформлен!";
                return RedirectToAction("List", "Order");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("Cart", "Basket");
            }
        }


        //  Список заказов пользователя
        public async Task<IActionResult> List()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Autorization", "Account");

            int userId = int.Parse(userIdClaim);

            var userResponse = await _httpClient.GetAsync($"/api/users/{userId}");
            if (!userResponse.IsSuccessStatusCode)
                return RedirectToAction("Autorization", "Account");

            var user = await userResponse.Content.ReadFromJsonAsync<User>();
            if (user == null)
                return RedirectToAction("Autorization", "Account");

            var ordersResponse = await _httpClient.GetAsync("/api/Orders");
            if (!ordersResponse.IsSuccessStatusCode)
                return View(new List<Order>());

            var orders = await ordersResponse.Content.ReadFromJsonAsync<List<Order>>();
            var userOrders = orders?.Where(o => o.UserId == user.IdUser).ToList();

            return View(userOrders ?? new List<Order>());
        }


        // Детали заказа
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Orders/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("List");

            var order = await response.Content.ReadFromJsonAsync<Order>();
            return View(order);
        }
    }
}
