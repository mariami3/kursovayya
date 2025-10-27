using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookMagazin.Models;

namespace Magazin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public ReviewsController(BookMagazinContext context)
        {
            _context = context;
        }

        // 🔹 Получить все отзывы (если нужно админке)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .ToListAsync();
        }

        // 🔹 Получить отзывы по книге
        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByBook(int bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return reviews;
        }

        // 🔹 Получить один отзыв
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.IdReview == id);

            if (review == null)
                return NotFound();

            return review;
        }

        // 🔹 Добавить отзыв
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            review.DateCreated = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.IdReview }, review);
        }

        // 🔹 Изменить отзыв
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.IdReview)
                return BadRequest();

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reviews.Any(e => e.IdReview == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // 🔹 Удалить отзыв
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
