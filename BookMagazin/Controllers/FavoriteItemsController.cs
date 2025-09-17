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
    public class FavoriteItemsController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public FavoriteItemsController(BookMagazinContext context)
        {
            _context = context;
        }

        // GET: api/FavoriteItems
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<FavoriteItem>>> GetUserFavorites(int userId)
        {
            var items = await _context.FavoriteItems
                .Include(f => f.Book)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            foreach (var item in items)
            {
                Console.WriteLine($"Книга ID: {item.BookId}, Название: {item.Book?.Title}");
            }

            return items;
        }

        // POST: api/FavoriteItems/AddToFavorites
        [HttpPost("AddToFavorites")]
        public async Task<ActionResult<FavoriteItem>> AddToFavorites([FromBody] AddToFavoritesRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound("Пользователь не найден");

            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
                return NotFound("Книга не найдена");

            var existingFavorite = await _context.FavoriteItems
                .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.BookId == request.BookId);

            if (existingFavorite != null)
                return Conflict("Книга уже в избранном");

            var favoriteItem = new FavoriteItem
            {
                UserId = request.UserId,
                BookId = request.BookId,
                Price = book.Price
            };

            _context.FavoriteItems.Add(favoriteItem);
            await _context.SaveChangesAsync();

            return Ok(favoriteItem);
        }

        // DELETE: api/FavoriteItems/RemoveFromFavorites/5
        [HttpDelete("RemoveFromFavorites/{id}")]
        public async Task<IActionResult> RemoveFromFavorites(int id)
        {
            var favoriteItem = await _context.FavoriteItems.FindAsync(id);
            if (favoriteItem == null)
                return NotFound();

            _context.FavoriteItems.Remove(favoriteItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class AddToFavoritesRequest
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
    }
}