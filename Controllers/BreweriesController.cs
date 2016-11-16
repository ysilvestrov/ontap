using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ontap.Auth;
using Ontap.Models;
using Ontap.Util;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    public class BreweriesController : Controller
    {
        private readonly DataContext _context;

        public BreweriesController(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Brewery> Breweries => _context.Breweries.Include(b => b.Country);


        // GET: api/pubs
        [HttpGet]
        public IEnumerable<Brewery> Get() => Breweries.ToArray();

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        [Authorize(Policy = "BreweryAdminUser")]
        public async Task<Brewery> Post([FromBody] Brewery brewery)
        {
            var pid = brewery.Name.MakeId();
            var id = pid;

            for (var i = 0; Breweries.Any(c => c.Id == id); id = $"{pid}_{i}", i++) ;

            brewery.Id = id;
            brewery.Country = _context.Countries.First(c => c.Id == brewery.Country.Id);
            _context.Breweries.Add(brewery);
            await _context.SaveChangesAsync();
            return brewery;
        }

        // PUT api/cities/Kharkiv
        [HttpPut("{id}")]
        [Authorize(Policy = "BreweryAdminUser")]
        public async Task<Brewery> Put(string id, [FromBody]Brewery brewery)
        {
            if (Breweries.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No beer with id {id}", id));
            var current = Breweries.First(c => c.Id == id);
            current.Name = brewery.Name;
            current.Address = brewery.Address;
            current.Image = brewery.Image;
            current.Country = _context.Countries.First(c => c.Id == brewery.Country.Id);
            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BreweryAdminUser")]
        public async Task Delete(string id)
        {
            if (Breweries.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No brewery with id {id}", id));
            if(_context.Beers.Any(b => b.Brewery.Id == id))
                throw new AlreadyExistsException("Cannot delete brewery with beers");
            _context.Breweries.Remove(_context.Breweries.First(c => c.Id == id));
            await _context.SaveChangesAsync();
        }
    }
}
