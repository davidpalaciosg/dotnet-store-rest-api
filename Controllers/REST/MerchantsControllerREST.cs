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
	public class MerchantsControllerREST : ControllerBase
	{
		private readonly StoreDbContext _context;

		public MerchantsControllerREST(StoreDbContext context)
		{
			_context = context;
		}

		// GET: api/MerchantsControllerREST
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Merchant>>> GetMerchants()
		{
			if (_context.Merchants == null)
			{
				return NotFound();
			}
			var completeMerchants = await _context.Merchants
				.Include(m => m.CountryCodeNavigation)
				.Include(m => m.Admin)
				.Where(m => m.State == 1)
				.ToListAsync();

			return completeMerchants;
		}

		// GET: api/MerchantsControllerREST/5
		[HttpGet("{id}")]
		public ActionResult<Merchant> GetMerchant(uint id)
		{
			if (_context.Merchants == null)
			{
				return NotFound();
			}
			var merchant = GetMerchantByID(id).Result;

			if (merchant == null || merchant.State == 0)
			{
				return NotFound();
			}

			return merchant;
		}

		// PUT: api/MerchantsControllerREST/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutMerchant(uint id, Merchant merchant)
		{
			if (id != merchant.Id)
			{
				return BadRequest();
			}

			_context.Entry(merchant).State = EntityState.Modified;

			try
			{
				merchant.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
				merchant.State = 1;
				_context.Merchants.Update(merchant);
				await _context.SaveChangesAsync();
				return Ok(merchant);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MerchantExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		// POST: api/MerchantsControllerREST
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Merchant>> PostMerchant(Merchant merchant)
		{
			if (_context.Merchants == null)
			{
				return Problem("Entity set 'StoreDbContext.Merchants'  is null.");
			}
			merchant.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

			//Find CountryCode
			var country = await _context.Countries.FindAsync(merchant.CountryCode);
			if (country == null || country.State == 0)
			{
				return NotFound("CountryCode not found.");
			}

			//Find Admin
			var admin = await _context.Users.FindAsync(merchant.AdminId);
			if (admin == null || admin.State == 0)
			{
				return NotFound("Admin not found.");
			}

			//Add fields
			merchant.CountryCodeNavigation = country;
			merchant.Admin = admin;

			//Save changes
			_context.Merchants.Add(merchant);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetMerchant", new { id = merchant.Id }, merchant);
		}

		// DELETE: api/MerchantsControllerREST/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMerchant(uint id)
		{
			if (_context.Merchants == null)
			{
				return NotFound();
			}

			var merchant = GetMerchantByID(id).Result;

			if (merchant == null)
			{
				return NotFound();
			}
			//Update the state of the merchant
			merchant.State = 0;
			_context.Entry(merchant).State = EntityState.Modified;
			_context.Merchants.Update(merchant);
			await _context.SaveChangesAsync();

			return Ok(merchant);
		}

		private bool MerchantExists(uint id)
		{
			return (_context.Merchants?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		private async Task<Merchant?> GetMerchantByID(uint id)
		{
			return await _context.Merchants
				.Include(m => m.CountryCodeNavigation)
				.Include(m => m.Admin)
				.FirstOrDefaultAsync(m => m.Id == id && m.State == 1);
		}
	}
}
