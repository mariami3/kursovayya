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
    public class BooksController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public BooksController(BookMagazinContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Adaptations)
                .Include(b => b.Reviews)
                .Include(b => b.CartItems)
                .Include(b => b.FavoriteItems)
                .Include(b => b.OrderItems)
                .Where(b => b.IsActive) // только активные книги
                .ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Adaptations)
                .Include(b => b.Reviews)
                .Include(b => b.CartItems)
                .Include(b => b.FavoriteItems)
                .Include(b => b.OrderItems)
                .FirstOrDefaultAsync(b => b.IdBook == id && b.IsActive);

            if (book == null)
                return NotFound();

            return book;
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.IdBook)
                return BadRequest("Id книги в URL не совпадает с Id в теле запроса.");

            try
            {
                // Проверяем, что книга вообще существует
                if (!BookExists(id))
                    return NotFound($"Книга с Id = {id} не найдена.");

                // Проверяем, что автор существует
                var authorExists = await _context.Authors.AnyAsync(a => a.IdAuthor == book.AuthorId);
                if (!authorExists)
                    return BadRequest($"Автор с Id = {book.AuthorId} не найден.");

                // Проверяем, что жанр существует
                var genreExists = await _context.Genres.AnyAsync(g => g.IdGenre == book.GenreId);
                if (!genreExists)
                    return BadRequest($"Жанр с Id = {book.GenreId} не найден.");

                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                    return NotFound($"Книга с Id = {id} больше не существует.");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении книги: {ex.Message}");
            }
        }


        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            try
            {
                // Проверяем, что есть такой автор
                var authorExists = await _context.Authors.AnyAsync(a => a.IdAuthor == book.AuthorId);
                if (!authorExists)
                    return BadRequest($"Автор с Id = {book.AuthorId} не найден.");

                // Проверяем, что есть такой жанр
                var genreExists = await _context.Genres.AnyAsync(g => g.IdGenre == book.GenreId);
                if (!genreExists)
                    return BadRequest($"Жанр с Id = {book.GenreId} не найден.");

                // По умолчанию книга активна
                book.IsActive = true;

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBook), new { id = book.IdBook }, book);
            }
            catch (Exception ex)
            {
                // тут мы вытащим вложенную ошибку SQL Server
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Ошибка при сохранении книги: {inner}");
            }
        }



        // DELETE: api/Books/5 — деактивация книги
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            book.IsActive = false; // помечаем как неактивную
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.IdBook == id);
        }

        // Controllers/BookController.cs
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularBooks([FromQuery] int topCount = 5)
        {
            try
            {
                var popularBooks = await _context.OrderItems
                    .Include(oi => oi.Book)
                    .ThenInclude(b => b.Author)
                    .GroupBy(oi => oi.BookId)
                    .Select(g => new
                    {
                        BookId = g.Key,
                        Title = g.First().Book.Title,
                        AuthorName = g.First().Book.Author.NameAuthor,
                        TotalOrders = g.Select(oi => oi.OrderId).Distinct().Count(),
                        TotalQuantity = g.Sum(oi => oi.Quantity),
                        TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .ThenByDescending(x => x.TotalOrders)
                    .Take(topCount)
                    .ToListAsync();

                return Ok(popularBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении статистики: {ex.Message}");
            }
        }
    }
}