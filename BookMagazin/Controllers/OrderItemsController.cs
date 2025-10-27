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
    public class OrderItemsController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public OrderItemsController(BookMagazinContext context)
        {
            _context = context;
        }

        // GET: api/OrderItems/Order/5
        [HttpGet("Order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetItemsByOrder(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Book)
                    .ThenInclude(b => b.Author)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        // POST: api/OrderItems
        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemsByOrder), new { orderId = orderItem.OrderId }, orderItem);
        }

        // PUT: api/OrderItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItem orderItem)
        {
            if (id != orderItem.IdOrderItem)
                return BadRequest();

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderItems.Any(e => e.IdOrderItem == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/OrderItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
                return NotFound();

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
