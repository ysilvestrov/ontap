using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ontap.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    public class BreweryAdminsController : Controller
    {
        private readonly DataContext _context;

        public BreweryAdminsController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<BreweryAdmin> Admins => _context.BreweryAdmins
            .Include(pa => pa.User)
            .Include(pa => pa.Brewery)
            .ToArray();


        // GET: api/serves
        [HttpGet]
        public IEnumerable<BreweryAdmin> Get() => Admins as BreweryAdmin[] ?? Admins.ToArray();

        // POST api/serves
        /// <exception cref="ArgumentException">Already existing record.</exception>
        [HttpPost]
        public async Task<BreweryAdmin> Post([FromBody] BreweryAdmin admin)
        {
            if (Admins.Any(s => s.Id == admin.Id))
                throw new ArgumentException(string.Format("Record with id {id} already exists", admin.Id));
            if (Admins.Any(s => s.User.Id == admin.User.Id && s.Brewery.Id == admin.Brewery.Id))
                throw new ArgumentException("Record for the same beer and pub already exists");
            var current = admin;
            current.User = _context.Users.First(user => user.Id == admin.User.Id);
            current.Brewery = _context.Breweries.First(brewery => brewery.Id == admin.Brewery.Id);
            _context.BreweryAdmins.Add(current);
            await _context.SaveChangesAsync();
            return admin;
        }

        // PUT api/serves/123
        /// <exception cref="KeyNotFoundException">Record not found.</exception>
        [HttpPut("{id}")]
        public async Task<BreweryAdmin> Put(int id, [FromBody]BreweryAdmin admin)
        { 
            if (Admins.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No record with id {id}", id));
            var current = Admins.First(c => c.Id == id);
            current.User = _context.Users.First(user => user.Id == admin.User.Id);
            current.Brewery = _context.Breweries.First(brewery => brewery.Id == admin.Brewery.Id);
            await _context.SaveChangesAsync();
            return current;
        }

        // Delete api/serves/123
        /// <exception cref="KeyNotFoundException">Recod not found.</exception>
        [HttpDelete("{id}")]
        public async Task<BreweryAdmin> Delete(int id)
        { 
            if (Admins.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No record with id {id}", id));
            var current = Admins.First(c => c.Id == id);
            _context.BreweryAdmins.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
