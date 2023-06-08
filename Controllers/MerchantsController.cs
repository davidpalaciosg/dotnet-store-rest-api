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
	public class MerchantsController : Controller
	{
		private readonly StoreDbContext _context;

		public MerchantsController(StoreDbContext context)
		{
			_context = context;
		}

		// GET: Merchants
		public async Task<IActionResult> Index()
		{
			var merchants = _context.Merchants
				.Include(m => m.Admin)
				.Include(m => m.CountryCodeNavigation)
				.Where(m => m.State == 1)
				.ToListAsync();
			return View(await merchants);
		}

		// GET: Merchants/Details/5
		public async Task<IActionResult> Details(uint? id)
		{
			if (id == null || _context.Merchants == null)
			{
				return NotFound();
			}

			var merchant = await _context.Merchants
				.Include(m => m.Admin)
				.Include(m => m.CountryCodeNavigation)
				.Where(m => m.State == 1)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (merchant == null || merchant.State == 0)
			{
				return NotFound();
			}

			return View(merchant);
		}

		// GET: Merchants/Create
		public IActionResult Create()
		{
			ViewData["AdminId"] = new SelectList(_context.Users.Where(u => u.State == 1), "Id", "Email");
			ViewData["CountryCode"] = new SelectList(_context.Countries.Where(c => c.State == 1), "Code", "Name");
			return View();
		}

		// POST: Merchants/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,MerchantName,AdminId,CountryCode,CreatedAt,State")] Merchant merchant)
		{
			//Add country code to merchant
			var country = await _context.Countries.FindAsync(merchant.CountryCode);
			if (country == null)
			{
				return NotFound("CountryCode is not valid");
			}

			//Add admin to merchant
			var admin = await _context.Users.FindAsync(merchant.AdminId);
			if (admin == null)
			{
				return NotFound("AdminId is not valid");
			}

			//Add merchant to database
			merchant.Admin = admin;
			merchant.CountryCodeNavigation = country;
			merchant.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

			_context.Add(merchant);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: Merchants/Edit/5
		public async Task<IActionResult> Edit(uint? id)
		{
			if (id == null || _context.Merchants == null)
			{
				return NotFound();
			}

			var merchant = await _context.Merchants.FindAsync(id);
			if (merchant == null || merchant.State == 0)
			{
				return NotFound();
			}
			ViewData["AdminId"] = new SelectList(_context.Users.Where(u => u.State == 1), "Id", "Email", merchant.AdminId);
			ViewData["CountryCode"] = new SelectList(_context.Countries.Where(c => c.State == 1), "Code", "Name", merchant.CountryCode);
			return View(merchant);
		}

		// POST: Merchants/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(uint id, [Bind("Id,MerchantName,AdminId,CountryCode,CreatedAt,State")] Merchant merchant)
		{
			if (id != merchant.Id)
			{
				return NotFound();
			}

			try
			{
				if (!MerchantExists(merchant.Id))
				{
					return NotFound();
				}
				_context.Update(merchant);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				throw;
			}
		}

		// GET: Merchants/Delete/5
		public async Task<IActionResult> Delete(uint? id)
		{
			if (id == null || _context.Merchants == null)
			{
				return NotFound();
			}

			var merchant = await _context.Merchants
				.Include(m => m.Admin)
				.Include(m => m.CountryCodeNavigation)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (merchant == null || merchant.State == 0)
			{
				return NotFound();
			}

			return View(merchant);
		}

		// POST: Merchants/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(uint id)
		{
			if (_context.Merchants == null)
			{
				return Problem("Entity set 'StoreDbContext.Merchants'  is null.");
			}
			var merchant = await _context.Merchants.FindAsync(id);
			if (merchant != null)
			{
				//Update merchant state
				merchant.State = 0;
				_context.Update(merchant);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool MerchantExists(uint id)
		{
			return (_context.Merchants?.Any(e => e.Id == id && e.State == 1)).GetValueOrDefault();
		}
	}
}
