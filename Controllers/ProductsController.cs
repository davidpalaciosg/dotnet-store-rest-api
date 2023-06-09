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
	public class ProductsController : Controller
	{
		private readonly StoreDbContext _context;

		public ProductsController(StoreDbContext context)
		{
			_context = context;
		}

		// GET: Products
		public async Task<IActionResult> Index()
		{
			var products = _context.Products
				.Include(p => p.Merchant)
				.Where(p => p.State == 1)
				.OrderBy(p => p.Merchant.MerchantName)
				.ThenBy(p => p.Name)
				.ToListAsync();

			return View(await products);
		}

		// GET: Products/Details/5
		public async Task<IActionResult> Details(uint? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Merchant)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null || product.State == 0)
			{
				return NotFound();
			}

			return View(product);
		}

		// GET: Products/Create
		public IActionResult Create()
		{
			ViewData["MerchantId"] = new SelectList(_context.Merchants.Where(m => m.State == true), "Id", "MerchantName");
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,MerchantId,Name,Price,Status,CreatedAt,State")] Product product)
		{

			//Get merchant
			var merchant = await _context.Merchants.FindAsync(product.MerchantId);
			if (merchant == null || merchant.State == false)
			{
				return NotFound("MerchantId is not valid");
			}

			//Add merchant to product
			product.Merchant = merchant;
			product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

			_context.Add(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: Products/Edit/5
		public async Task<IActionResult> Edit(uint? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null || product.State == 0)
			{
				return NotFound();
			}
			ViewData["MerchantId"] = new SelectList(_context.Merchants.Where(m => m.State == true), "Id", "MerchantName", product.MerchantId);
			return View(product);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(uint id, [Bind("Id,MerchantId,Name,Price,Status,CreatedAt,State")] Product product)
		{
			if (id != product.Id)
			{
				return NotFound();
			}
			try
			{
				if (!ProductExists(product.Id))
				{
					return NotFound();
				}
				_context.Update(product);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				throw;
			}
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> Delete(uint? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Merchant)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null || product.State == 0)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(uint id)
		{
			if (_context.Products == null)
			{
				return Problem("Entity set 'StoreDbContext.Products'  is null.");
			}
			var product = await _context.Products.FindAsync(id);
			if (product != null)
			{
				//Update product state
				product.State = 0;
				_context.Update(product);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProductExists(uint id)
		{
			return (_context.Products?.Any(e => e.Id == id && e.State == 1)).GetValueOrDefault();
		}
	}
}
