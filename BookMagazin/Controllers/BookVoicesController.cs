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
    public class BookVoicesController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public BookVoicesController(BookMagazinContext context)
        {
            _context = context;
        }

        // GET: api/BookVoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookVoice>>> GetBookVoices()
        {
            return await _context.BookVoices
                                 .Include(v => v.Book)
                                 .ToListAsync();
        }

        // GET: api/BookVoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookVoice>> GetBookVoice(int id)
        {
            var bookVoice = await _context.BookVoices
                                          .Include(v => v.Book)
                                          .FirstOrDefaultAsync(v => v.IdVoice == id);

            if (bookVoice == null)
                return NotFound();

            return bookVoice;
        }

        // GET: api/BookVoices/ByBook/3
        [HttpGet("ByBook/{bookId}")]
        public async Task<ActionResult<IEnumerable<BookVoice>>> GetVoicesByBook(int bookId)
        {
            var voices = await _context.BookVoices
                                       .Where(v => v.BookId == bookId)
                                       .Include(v => v.Book)
                                       .ToListAsync();

            if (voices == null || !voices.Any())
                return NotFound(new { message = "Озвучки для этой книги не найдены." });

            return voices;
        }

        // PUT: api/BookVoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookVoice(int id, [FromForm] int bookId, [FromForm] string title, [FromForm] int? durationSeconds, [FromForm] string? format, [FromForm] IFormFile? audioFile)
        {
            var existingVoice = await _context.BookVoices.FindAsync(id);
            if (existingVoice == null)
                return NotFound();

            // Обновляем основные поля
            existingVoice.BookId = bookId;
            existingVoice.Title = title;
            existingVoice.DurationSeconds = durationSeconds;
            existingVoice.Format = format;

            // Если передан новый файл, обновляем его
            if (audioFile != null && audioFile.Length > 0)
            {
                // Проверяем расширение
                var extension = Path.GetExtension(audioFile.FileName);
                if (extension != ".mp3" && extension != ".wav" && extension != ".ogg")
                    return BadRequest("Допустимы только аудиофайлы (.mp3, .wav, .ogg).");

                // Путь сохранения
                var uploadPath = @"C:\BookAudio";
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await audioFile.CopyToAsync(stream);
                }

                // Удаляем старый файл
                if (!string.IsNullOrEmpty(existingVoice.VoiceUrl))
                {
                    var oldFileName = Path.GetFileName(existingVoice.VoiceUrl);
                    var oldFullPath = Path.Combine(uploadPath, oldFileName);
                    if (System.IO.File.Exists(oldFullPath))
                    {
                        System.IO.File.Delete(oldFullPath);
                    }
                }

                existingVoice.VoiceUrl = $"/audio/{fileName}";
                existingVoice.Format = extension.Trim('.');
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookVoiceExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/BookVoices
        [HttpPost]
        public async Task<ActionResult<BookVoice>> PostBookVoice([FromForm] int bookId, [FromForm] string title, [FromForm] int? durationSeconds, [FromForm] string? format, [FromForm] IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest("Файл не выбран.");

            // Проверяем расширение
            var extension = Path.GetExtension(audioFile.FileName);
            if (extension != ".mp3" && extension != ".wav" && extension != ".ogg")
                return BadRequest("Допустимы только аудиофайлы (.mp3, .wav, .ogg).");

            // Путь сохранения
            var uploadPath = @"C:\BookAudio";
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

            var bookVoice = new BookVoice
            {
                BookId = bookId,
                Title = title,
                VoiceUrl = $"/audio/{fileName}",
                DurationSeconds = durationSeconds,
                Format = format ?? extension.Trim('.')
            };

            _context.BookVoices.Add(bookVoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookVoice), new { id = bookVoice.IdVoice }, bookVoice);
        }

        // DELETE: api/BookVoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookVoice(int id)
        {
            var bookVoice = await _context.BookVoices.FindAsync(id);
            if (bookVoice == null)
                return NotFound();

            // Удаляем файл
            if (!string.IsNullOrEmpty(bookVoice.VoiceUrl))
            {
                var uploadPath = @"C:\BookAudio";
                var fileName = Path.GetFileName(bookVoice.VoiceUrl);
                var fullPath = Path.Combine(uploadPath, fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.BookVoices.Remove(bookVoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookVoiceExists(int id)
        {
            return _context.BookVoices.Any(e => e.IdVoice == id);
        }
    }
}