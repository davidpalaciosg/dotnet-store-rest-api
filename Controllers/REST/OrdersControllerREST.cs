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
	public class OrdersControllerREST : ControllerBase
	{
		private readonly StoreDbContext _context;

		public OrdersControllerREST(StoreDbContext context)
		{
			_context = context;
		}

		// GET: api/OrdersControllerREST
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
		{
			if (_context.Orders == null)
			{
				return NotFound();
			}
			var completeOrders = await _context.Orders
				.Include(o => o.User)
				.Include(o => o.OrderItems)
				.Where(o => o.State == 1)
				.ToListAsync();

			return completeOrders;
		}

		// GET: api/OrdersControllerREST/5
		[HttpGet("{id}")]
		public ActionResult<Order> GetOrder(uint id)
		{
			if (_context.Orders == null)
			{
				return NotFound();
			}
			var order = getOrderById(id).Result;

			if (order == null || order.State == 0)
			{
				return NotFound();
			}

			return order;
		}

		// PUT: api/OrdersControllerREST/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutOrder(uint id, Order order)
		{
			if (id != order.Id)
			{
				return BadRequest();
			}

			_context.Entry(order).State = EntityState.Modified;

			try
			{
				order.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
				order.State = 1;
				order.User = await _context.Users.FindAsync(order.UserId);
				_context.Orders.Update(order);
				await _context.SaveChangesAsync();
				return Ok(order);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!OrderExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		// POST: api/OrdersControllerREST
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Order>> PostOrder(Order order)
		{
			if (_context.Orders == null)
			{
				return Problem("Entity set 'StoreDbContext.Orders'  is null.");
			}

			//Find User
			var user = await _context.Users.FindAsync(order.UserId);
			if(user==null || user.State==0)
			{
				return NotFound("UserId not found.");
			}

			//Add fields
			order.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
			order.State = 1;
			order.User = user;

			//Save changes
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetOrder", new { id = order.Id }, order);
		}

		// DELETE: api/OrdersControllerREST/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteOrder(uint id)
		{
			if (_context.Orders == null)
			{
				return NotFound();
			}
			var order = getOrderById(id).Result;
			if (order == null || order.State == 0)
			{
				return NotFound();
			}

			//Update the state of the order
			order.State = 0;
			order.Status = "ELIMINADO";
			order.User = await _context.Users.FindAsync(order.UserId);
			_context.Entry(order).State = EntityState.Modified;
			_context.Orders.Update(order);
			await _context.SaveChangesAsync();

			return Ok(order);
		}

		private bool OrderExists(uint id)
		{
			return (_context.Orders?.Any(e => e.Id == id && e.State==1)).GetValueOrDefault();
		}

		private Task<Order?> getOrderById(uint id)
		{
			return _context.Orders.
				Include(o => o.User).
				Include(o => o.OrderItems).
				FirstOrDefaultAsync(o => o.Id == id && o.State == 1);
		}
	}
}
