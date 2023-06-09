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
	public class OrdersController : Controller
	{
		private readonly StoreDbContext _context;

		public OrdersController(StoreDbContext context)
		{
			_context = context;
		}

		// GET: Orders
		public async Task<IActionResult> Index()
		{
			var orders = _context.Orders
				 .Include(o => o.User)
				 .ToListAsync();
			return View(await orders);
		}

		// GET: Orders/Details/5
		public async Task<IActionResult> Details(uint? id)
		{
			if (id == null || _context.Orders == null)
			{
				return NotFound();
			}

			var order = await _context.Orders
				.Include(o => o.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (order == null || order.State == 0)
			{
				return NotFound();
			}

			return View(order);
		}

		// GET: Orders/Create
		public IActionResult Create()
		{
			ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.State == 1), "Id", "Email");
			return View();
		}

		// POST: Orders/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,UserId,Status,CreatedAt,State")] Order order)
		{
			//Get the current user
			var user = await _context.Users.FindAsync(order.UserId);
			if (user == null || user.State == 0)
			{
				return NotFound("UserId is not valid");
			}

			//Add user to order
			order.User = user;
			order.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

			_context.Add(order);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: Orders/Edit/5
		public async Task<IActionResult> Edit(uint? id)
		{
			if (id == null || _context.Orders == null)
			{
				return NotFound();
			}

			var order = await _context.Orders.FindAsync(id);
			if (order == null || order.State == 0)
			{
				return NotFound();
			}
			ViewData["UserId"] = new SelectList(_context.Users.Where(u=>u.State==1), "Id", "Email", order.UserId);
			return View(order);
		}

		// POST: Orders/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(uint id, [Bind("Id,UserId,Status,CreatedAt,State")] Order order)
		{
			if (id != order.Id)
			{
				return NotFound();
			}
			try
			{
				if (!OrderExists(order.Id))
				{
					return NotFound();
				}
				_context.Update(order);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				throw;
			}
		}

		// GET: Orders/Delete/5
		public async Task<IActionResult> Delete(uint? id)
		{
			if (id == null || _context.Orders == null)
			{
				return NotFound();
			}

			var order = await _context.Orders
				.Include(o => o.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		// POST: Orders/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(uint id)
		{
			if (_context.Orders == null)
			{
				return Problem("Entity set 'StoreDbContext.Orders'  is null.");
			}
			var order = await _context.Orders.FindAsync(id);
			if (order != null)
			{
				//Update the state of the order to 0
				order.State = 0;
				_context.Update(order);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool OrderExists(uint id)
		{
			return (_context.Orders?.Any(e => e.Id == id && e.State == 1)).GetValueOrDefault();
		}
	}
}
