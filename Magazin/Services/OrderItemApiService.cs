using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Magazin.Models;

namespace Magazin.Services
{
    public class OrderItemApiService
    {
        private readonly HttpClient _http;

        public OrderItemApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task AddOrderItemAsync(OrderItem orderItem)
        {
            await _http.PostAsJsonAsync("OrderItems", orderItem);
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderAsync(int orderId)
        {
            return await _http.GetFromJsonAsync<List<OrderItem>>($"OrderItems/Order/{orderId}");
        }

        public async Task<bool> RemoveOrderItemAsync(int orderItemId)
        {
            var response = await _http.DeleteAsync($"OrderItems/{orderItemId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateOrderItemAsync(OrderItem item)
        {
            var response = await _http.PutAsJsonAsync($"OrderItems/{item.IdOrderItem}", item);
            return response.IsSuccessStatusCode;
        }
    }
}
