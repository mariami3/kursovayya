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
    public class OrdersController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public OrdersController(BookMagazinContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                        .ThenInclude(b => b.Author)
                .Include(o => o.User)
                .ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                        .ThenInclude(b => b.Author)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.IdOrder == id);

            if (order == null)
                return NotFound();

            return order;
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.IdOrder)
                return BadRequest();

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.IdOrder == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (order == null || order.OrderItems == null || !order.OrderItems.Any())
                return BadRequest("Заказ пустой или без позиций.");

            var user = await _context.Users.FindAsync(order.UserId);
            if (user == null)
                return BadRequest("Пользователь не найден.");

            order.User = user;

            foreach (var item in order.OrderItems)
            {
                if (!_context.Books.Any(b => b.IdBook == item.BookId))
                    return BadRequest($"Книга с Id {item.BookId} не найдена.");
                item.Order = order; // привязка к заказу
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.IdOrder }, order);
        }




        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.IdOrder == id);

            if (order == null)
                return NotFound();

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
