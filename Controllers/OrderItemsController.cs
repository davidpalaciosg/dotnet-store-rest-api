using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dotnet_products_rest_api.Models;

namespace dotnet_products_rest_api.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly StoreDbContext _context;

        public OrderItemsController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            var items = _context.OrderItems
                .Include(o => o.Order)
                    .ThenInclude(o1 => o1.User)
                .Include(o => o.Product)
                .OrderBy(o => o.OrderId)
                .ThenBy(o => o.Product.Name)
                .Where(o => o.State == 1)
                .ToListAsync();
            return View(await items);
        }

        // GET: OrderItems/Details/5/10
        public async Task<IActionResult> Details(uint? orderId, uint? productId)
        {
            if (orderId == null || productId == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == orderId && m.ProductId == productId);

            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }


        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders.Where(o => o.State == 1), "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.State == 1), "Id", "Name");
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,Quantity,State")] OrderItem orderItem)
        {
            //Check if exists a orderItem with the same orderId and productId
            var orderItemExists = await _context.OrderItems
                .FirstAsync(o => o.OrderId == orderItem.OrderId && o.ProductId == orderItem.ProductId);
            //If exists then update the quantity and state
            if (orderItemExists != null)
            {
                orderItemExists.State = 1;
                orderItemExists.Quantity = orderItem.Quantity;
                _context.Update(orderItemExists);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //Get the product
            var product = await _context.Products.FindAsync(orderItem.ProductId);
            if (product == null || product.State == 0)
            {
                return NotFound("ProductId is not valid");
            }

            //Get the order
            var order = await _context.Orders.FindAsync(orderItem.OrderId);
            if (order == null || order.State == 0)
            {
                return NotFound("OrderId is not valid");
            }

            //Add fields
            orderItem.State = 1;
            orderItem.Product = product;
            orderItem.Order = order;

            //Save changes
            _context.Add(orderItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        // GET: OrderItems/Edit/5/10
        public async Task<IActionResult> Edit(uint? orderId, uint? productId)
        {
            if (orderId == null || productId == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.OrderId == orderId && m.ProductId == productId);

            if (orderItem == null || orderItem.State == 0)
            {
                return NotFound();
            }

            ViewData["OrderId"] = new SelectList(_context.Orders.Where(o => o.State == 1), "Id", "Id", orderItem.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.State == 1), "Id", "Name", orderItem.ProductId);
            return View(orderItem);
        }


        // POST: OrderItems/Edit/5/10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint orderId, uint productId, [Bind("OrderId,ProductId,Quantity,State")] OrderItem orderItem)
        {
            if (orderId != orderItem.OrderId || productId != orderItem.ProductId)
            {
                return NotFound();
            }

            try
            {
                if (!OrderItemExists(orderItem.OrderId, orderItem.ProductId))
                {
                    return NotFound();
                }

                orderItem.State = 1;
                _context.Update(orderItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItem.OrderId);
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", orderItem.ProductId);

                return View(orderItem);
            }
        }

        // GET: OrderItems/Delete/5/10
        public async Task<IActionResult> Delete(uint? orderId, uint? productId)
        {
            if (orderId == null || productId == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == orderId && m.ProductId == productId);

            if (orderItem == null || orderItem.State == 0)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5/10
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(uint orderId, uint productId)
        {
            if (_context.OrderItems == null)
            {
                return Problem("Entity set 'StoreDbContext.OrderItems' is null.");
            }

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.OrderId == orderId && m.ProductId == productId);

            if (orderItem != null)
            {
                // Update order item state
                orderItem.State = 0;
                _context.Update(orderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool OrderItemExists(uint? orderId, uint? productId)
        {
            return _context.OrderItems?.Any(e => e.OrderId == orderId && e.ProductId == productId && e.State == 1) ?? false;
        }

    }
}
