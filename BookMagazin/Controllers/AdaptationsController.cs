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
    public class AdaptationsController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public AdaptationsController(BookMagazinContext context)
        {
            _context = context;
        }

        // GET: api/Adaptations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Adaptation>>> GetAdaptations()
        {
            return await _context.Adaptations
                                 .Include(a => a.Book) // включаем данные о книге
                                 .ToListAsync();
        }

        // GET: api/Adaptations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Adaptation>> GetAdaptation(int id)
        {
            var adaptation = await _context.Adaptations
                                           .Include(a => a.Book)
                                           .FirstOrDefaultAsync(a => a.IdAdaptation == id);

            if (adaptation == null)
                return NotFound();

            return adaptation;
        }

        // PUT: api/Adaptations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdaptation(int id, Adaptation adaptation)
        {
            if (id != adaptation.IdAdaptation)
                return BadRequest();

            _context.Entry(adaptation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdaptationExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Adaptations
        [HttpPost]
        public async Task<ActionResult<Adaptation>> PostAdaptation(Adaptation adaptation)
        {
            _context.Adaptations.Add(adaptation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdaptation), new { id = adaptation.IdAdaptation }, adaptation);
        }

        // DELETE: api/Adaptations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdaptation(int id)
        {
            var adaptation = await _context.Adaptations
                                           .Include(a => a.Book)
                                           .FirstOrDefaultAsync(a => a.IdAdaptation == id);

            if (adaptation == null)
                return NotFound();

            _context.Adaptations.Remove(adaptation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdaptationExists(int id)
        {
            return _context.Adaptations.Any(e => e.IdAdaptation == id);
        }
    }
}
