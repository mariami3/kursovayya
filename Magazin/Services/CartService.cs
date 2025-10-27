using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Magazin.Models;

namespace Magazin.Services
{
    public class CartService
    {
        private readonly HttpClient _http;

        public CartService(HttpClient http)
        {
            _http = http;
        }

        public async Task AddToCartAsync(int userId, int bookId, int quantity = 1, decimal price = 0)
        {
            var cartItem = new CartItem
            {
                UserId = userId,
                BookId = bookId,
                Quantity = quantity,
                Price = price
            };

            await _http.PostAsJsonAsync("CartItems", cartItem);
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            return await _http.GetFromJsonAsync<List<CartItem>>($"CartItems/User/{userId}");
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            var response = await _http.DeleteAsync($"CartItems/{cartItemId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCartItemAsync(CartItem item)
        {
            var response = await _http.PutAsJsonAsync($"CartItems/{item.IdCartItem}", item);
            return response.IsSuccessStatusCode;
        }
    }
}
