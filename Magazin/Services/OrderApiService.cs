using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Magazin.Models;

namespace Magazin.Services
{
    public class OrderApiService
    {
        private readonly HttpClient _http;

        public OrderApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _http.PostAsJsonAsync("Orders", order);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _http.GetFromJsonAsync<Order>($"Orders/{orderId}");
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _http.GetFromJsonAsync<List<Order>>($"Orders/User/{userId}");
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _http.GetFromJsonAsync<List<Order>>("Orders");
        }

        public async Task<bool> RemoveOrderAsync(int orderId)
        {
            var response = await _http.DeleteAsync($"Orders/{orderId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var response = await _http.PutAsJsonAsync($"Orders/{order.IdOrder}", order);
            return response.IsSuccessStatusCode;
        }


    }
}
