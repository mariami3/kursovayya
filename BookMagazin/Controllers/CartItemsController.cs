using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookMagazin.Models;

namespace BookMagazin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public CartItemsController(BookMagazinContext context)
        {
            _context = context;
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetUserCartItems(int userId)
        {
            var items = await _context.CartItems
                .Include(ci => ci.Book)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            return items;
        }


        [HttpPost("AddToCart")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
            {
                return NotFound("Книга не найдена");
            }

            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.UserId == request.UserId &&
                    ci.BookId == request.BookId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += request.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = request.UserId,
                    BookId = request.BookId,
                    Quantity = request.Quantity,
                    Price = book.Price
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public class AddToCartRequest
        {
            public int UserId { get; set; }
            public int BookId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpPut("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateQuantityRequest request)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Количество должно быть больше нуля");
            }

            cartItem.Quantity = request.Quantity;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class UpdateQuantityRequest
        {
            public int Quantity { get; set; }
        }

        [HttpDelete("RemoveFromCart/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("TotalSum/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalSum(int userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Book)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            var totalSum = cartItems.Sum(ci => (ci.Book?.Price ?? 0) * ci.Quantity);

            return totalSum;
        }
    }
}