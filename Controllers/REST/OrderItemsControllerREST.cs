using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_products_rest_api.Models;

namespace dotnet_products_rest_api.Controllers.REST
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsControllerREST : ControllerBase
    {
        private readonly StoreDbContext _context;

        public OrderItemsControllerREST(StoreDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderItemsControllerREST
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            if (_context.OrderItems == null)
            {
                return NotFound();
            }
            var items = _context.OrderItems
                            .Where(o => o.State == 1)
                            .ToListAsync();
            return await items;
        }

        // GET: api/OrderItemsControllerREST/5/10
        [HttpGet("{orderId}/{productId}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(uint? orderId, uint? productId)
        {
            if (_context.OrderItems == null || orderId == null || productId == null)
            {
                return NotFound("orderId or productId are not valid");
            }


            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.OrderId == orderId && m.ProductId == productId);

            if (orderItem == null || orderItem.State == 0)
            {
                return NotFound();
            }

            return orderItem;
        }

        // PUT: api/OrderItemsControllerREST/5/10
        [HttpPut("{orderId}/{productId}")]
        public async Task<IActionResult> PutOrderItem(uint orderId, uint productId, OrderItem orderItem)
        {
            if (orderId != orderItem.OrderId || productId != orderItem.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                if (!OrderItemExists(orderId, productId))
                {
                    return NotFound();
                }
                orderItem.State = 1;
                _context.Update(orderItem);
                await _context.SaveChangesAsync();
                return Ok(orderItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound("orderId or productId not found");
            }
        }


        // POST: api/OrderItemsControllerREST
        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem? orderItem)
        {
            if (_context.OrderItems == null || orderItem == null)
            {
                return Problem("Entity set 'StoreDbContext.OrderItems'  is null.");
            }

            //Check if exists a orderItem with the same orderId and productId
            var orderItemExists = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.OrderId == orderItem.OrderId && m.ProductId == orderItem.ProductId);

            //If exists, update the quantity and state
            if (orderItemExists != null)
            {
                orderItemExists.State = 1;
                orderItemExists.Quantity = orderItem.Quantity;
                _context.Entry(orderItemExists).State = EntityState.Modified;
                _context.Update(orderItemExists);
                await _context.SaveChangesAsync();
                return orderItemExists;
            }

            //Get the order
            var order = await _context.Orders.FindAsync(orderItem.OrderId);
            if (order == null || order.State == 0)
            {
                return NotFound("OrderId is not valid");
            }

            //Get the product
            var product = await _context.Products.FindAsync(orderItem.ProductId);
            if (product == null || product.State == 0)
            {
                return NotFound("ProductId is not valid");
            }

            //Add fields
            orderItem.State = 1;
            orderItem.Order = order;
            orderItem.Product = product;

            //Save changes
            _context.Add(orderItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOrderItem", new { id = orderItem.OrderId }, orderItem);
        }

        // DELETE: api/OrderItemsControllerREST/5
        [HttpDelete("{orderId}/{productId}")]
        public async Task<IActionResult> DeleteOrderItem(uint? orderId, uint? productId)
        {
            if (_context.OrderItems == null || orderId == null || productId == null)
            {
                return NotFound("orderId or productId are not valid");
            }

            var orderItem = await _context.OrderItems.FirstOrDefaultAsync(m => m.OrderId == orderId && m.ProductId == productId);
            if (orderItem == null)
            {
                return NotFound();
            }
            orderItem.State = 0;
            _context.Entry(orderItem).State = EntityState.Modified;
            _context.Update(orderItem);
            await _context.SaveChangesAsync();

            return Ok(orderItem);
        }

        private bool OrderItemExists(uint orderId, uint productId)
        {
            return (_context.OrderItems?.Any(e => e.OrderId == orderId && e.ProductId == productId && e.State == 1)).GetValueOrDefault();
        }
    }
}
