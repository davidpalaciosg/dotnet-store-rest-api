using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_products_rest_api.Models;

namespace dotnet_products_rest_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersControllerREST : ControllerBase
	{
		private readonly StoreDbContext _context;

		public UsersControllerREST(StoreDbContext context)
		{
			_context = context;
		}

		// GET: api/Users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			if (_context.Users == null)
			{
				return NotFound();
			}

			var completeUsers = await _context.Users
				  .Include(u => u.CountryCodeNavigation)
				  .Where(u => u.State == 1)
				  .ToListAsync();

			return completeUsers;
		}

		// GET: api/Users/5
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUser(uint id)
		{
			if (_context.Users == null)
			{
				return NotFound();
			}

			var user = await _context.Users
				.Include(u => u.CountryCodeNavigation)
				.Where(u => u.Id == id && u.State == 1)
				.FirstOrDefaultAsync();

			if (user == null || user.State == 0)
			{
				return NotFound();
			}

			return user;
		}

		// PUT: api/Users/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUser(uint id, User user)
		{
			if (id != user.Id)
			{
				return BadRequest();
			}

			_context.Entry(user).State = EntityState.Modified;

			try
			{
				user.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
				_context.Users.Update(user);
				await _context.SaveChangesAsync();
				return Ok(user);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		// POST: api/Users
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<User>> PostUser(User user)
		{
			if (_context.Users == null)
			{
				return Problem("Entity set 'StoreDbContext.Users'  is null.");
			}

			user.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
			//Find and Add country
			var c = await _context.Countries.FindAsync(user.CountryCode);
			if (c == null)
				return BadRequest("Country with code " + user.CountryCode + " not found.");


			user.CountryCodeNavigation = c;

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetUser", new { id = user.Id }, user);
		}

		// DELETE: api/Users/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(uint id)
		{
			if (_context.Users == null)
			{
				return NotFound();
			}
			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			//Update the state of the user to 0
			user.State = 0;
			_context.Entry(user).State = EntityState.Modified;
			_context.Users.Update(user);
			await _context.SaveChangesAsync();

			return Ok(user);
		}

		private bool UserExists(uint id)
		{
			return (_context.Users?.Any(e => e.Id == id && e.State == 1)).GetValueOrDefault();
		}
	}
}
