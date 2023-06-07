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
    public class CountriesControllerREST : ControllerBase
    {
        private readonly StoreDbContext _context;

        public CountriesControllerREST(StoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            var filteredCountries = await _context.Countries.Where(c => c.State == 1).ToListAsync();
            return filteredCountries;
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(uint id)
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            var country = await _context.Countries.FindAsync(id);

            if (country == null || country.State == 0)
            {
                return NotFound();
            }

            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(uint id, Country country)
        {
            if (id != country.Code)
            {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'StoreDbContext.Countries'  is null.");
            }
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Code }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(uint id)
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            var country = await _context.Countries.FindAsync(id);
            if (country == null || country.State == 0)
            {
                return NotFound();
            }
            //Update the state of the country to 0
            country.State = 0;
            _context.Entry(country).State = EntityState.Modified;
            _context.Countries.Update(country);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(uint id)
        {
            return (_context.Countries?.Any(e => e.Code == id && e.State == 1)).GetValueOrDefault();
        }
    }
}
