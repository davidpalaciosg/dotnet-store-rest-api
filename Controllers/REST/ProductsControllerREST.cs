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
	public class ProductsControllerREST : ControllerBase
	{
		private readonly StoreDbContext _context;

		public ProductsControllerREST(StoreDbContext context)
		{
			_context = context;
		}

		// GET: api/ProductsControllerREST
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			if (_context.Products == null)
			{
				return NotFound();
			}
			var completeProducts = await _context.Products
				.Where(p => p.State == 1)
				.ToListAsync();
			return completeProducts;
		}

		// GET: api/ProductsControllerREST/5
		[HttpGet("{id}")]
		public ActionResult<Product> GetProduct(uint id)
		{
			if (_context.Products == null)
			{
				return NotFound();
			}
			var product = getProductById(id).Result;

			if (product == null || product.State == 0)
			{
				return NotFound();
			}

			return product;
		}

		// PUT: api/ProductsControllerREST/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutProduct(uint id, Product product)
		{
			if (id != product.Id)
			{
				return BadRequest();
			}

			_context.Entry(product).State = EntityState.Modified;

			try
			{
				product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
				product.State = 1;
				product.Merchant = await _context.Merchants.FindAsync(product.MerchantId);
				_context.Products.Update(product);
				await _context.SaveChangesAsync();
				return Ok(product);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		// POST: api/ProductsControllerREST
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Product>> PostProduct(Product product)
		{
			if (_context.Products == null)
			{
				return Problem("Entity set 'StoreDbContext.Products'  is null.");
			}
			//Find Merchant
			var merchant = await _context.Merchants.FindAsync(product.MerchantId);
			if (merchant == null || merchant.State == 0)
			{
				return NotFound("MerchantId not found.");
			}

			//Add fields
			product.Merchant = merchant;
			product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

			//Save changes
			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProduct", new { id = product.Id }, product);
		}

		// DELETE: api/ProductsControllerREST/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(uint id)
		{
			if (_context.Products == null)
			{
				return NotFound();
			}
			var product = getProductById(id).Result;
			if (product == null || product.State == 0)
			{
				return NotFound();
			}
			//Update the state of the product
			product.State = 0;
			_context.Entry(product).State = EntityState.Modified;
			_context.Products.Update(product);
			await _context.SaveChangesAsync();
			return Ok(product);
		}

		private bool ProductExists(uint id)
		{
			return (_context.Products?.Any(e => e.Id == id && e.State == 1)).GetValueOrDefault();
		}

		private async Task<Product?> getProductById(uint? id)
		{
			return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.State == 1);
		}
	}
}
