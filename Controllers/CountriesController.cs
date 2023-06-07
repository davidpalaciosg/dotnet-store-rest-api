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
    public class CountriesController : Controller
    {
        private readonly StoreDbContext _context;

        public CountriesController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            var countries = _context.Countries;

            if (countries == null)
            {
                return Problem("Entity set 'StoreDbContext.Countries' is null.");
            }
            
            var filteredCountries = await countries.Where(c => c.State == 1).ToListAsync();
            return View(filteredCountries);
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(uint? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Code == id && m.State == 1);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,ContinentName,State")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(uint? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, [Bind("Code,Name,ContinentName,State")] Country country)
        {
            if (id != country.Code)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Code))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(uint? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Code == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(uint id)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'StoreDbContext.Countries'  is null.");
            }
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                //Update the state of the country to 0
                country.State = 0;
                _context.Countries.Update(country);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(uint id)
        {
            return (_context.Countries?.Any(e => e.Code == id && e.State == 1)).GetValueOrDefault();
        }
    }
}
