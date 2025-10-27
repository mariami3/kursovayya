using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Magazin.Services;

namespace Magazin.Controllers
{
    [Authorize(Roles = "Менеджер")]
    public class ManagerController : Controller
    {
        private readonly OrderApiService _orderApiService;
        private readonly OrderItemApiService _orderItemApiService;

        public ManagerController(OrderApiService orderApiService, OrderItemApiService orderItemApiService)
        {
            _orderApiService = orderApiService;
            _orderItemApiService = orderItemApiService;
        }

        // Аналитика: статистика заказов
        public async Task<IActionResult> Analytics()
        {
            var orders = await _orderApiService.GetAllOrdersAsync();

            var totalOrders = orders.Count;
            var totalSum = orders.Sum(o => o.TotalSum);
            var completed = orders.Count(o => o.StatusOrders == "Доставлен");

            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalSum = totalSum;
            ViewBag.CompletedOrders = completed;

            return View(orders);
        }

        // Список заказов
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderApiService.GetAllOrdersAsync();
            return View(orders);
        }

        // Детали заказа
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderApiService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // Изменение статуса заказа
        // GET: Форма изменения статуса
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var order = await _orderApiService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Сохранение изменения статуса 
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var order = await _orderApiService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            order.StatusOrders = newStatus;
            var updated = await _orderApiService.UpdateOrderAsync(order);

            if (!updated)
                TempData["Error"] = "Ошибка при изменении статуса.";
            else
                TempData["Success"] = "Статус заказа обновлен.";

            return RedirectToAction("Orders");
        }

    }
}
