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
    public class UsersController : Controller
    {
        private readonly StoreDbContext _context;

        public UsersController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var storeDbContext = _context.Users.Include(u => u.CountryCodeNavigation)
                .Where(u => u.State == 1);
            return View(await storeDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(uint? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null || user.State == 0)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["CountryCode"] = new SelectList(_context.Countries.Where(c => c.State == 1), "Code", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Email,Gender,DateOfBirth,CountryCode,CreatedAt,State")] User user)
        {
            //Add country code navigation property
            var country = await _context.Countries.FindAsync(user.CountryCode);
            if (country == null)
                return NotFound("Country code is not valid.");

            user.CountryCodeNavigation = country;

            //Add CreatedAt property
            user.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(uint? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null || user.State == 0)
            {
                return NotFound();
            }
            ViewData["CountryCode"] = new SelectList(_context.Countries.Where(c => c.State == 1), "Code", "Name");
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, [Bind("Id,FullName,Email,Gender,DateOfBirth,CountryCode,CreatedAt,State")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            try
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(uint? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null || user.State == 0)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(uint id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'StoreDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                //Update state to deleted
                user.State = 0;
                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(uint id)
        {
            return (_context.Users?.Any(e => e.Id == id && e.State == 1)).GetValueOrDefault();
        }
    }
}
