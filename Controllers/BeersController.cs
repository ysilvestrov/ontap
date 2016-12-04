using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ontap.Auth;
using Ontap.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "BreweryOrPubAdminUser")]
    public class BeersController : Controller
    {
        private readonly DataContext _context;

        public BeersController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<Beer> Beers => _context.Beers.Include(b => b.Brewery);


        // GET: api/pubs
        [HttpGet]
        public IEnumerable<Beer> Get() => Beers.ToArray();

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        public async Task<Beer> Post([FromBody] Beer beer)
        {
            if (Beers.Any(c => c.Id == beer.Id))
                throw new AlreadyExistsException(string.Format("Beer with id {id} already exists", beer.Id));
            beer.Brewery = _context.Breweries.First(b => b.Id == beer.Brewery.Id);
            _context.Beers.Add(beer);
            await _context.SaveChangesAsync();
            return beer;
        }

        // PUT api/cities/Kharkiv
        [HttpPut("{id}")]
        public async Task<Beer> Put(string id, [FromBody]Beer beer)
        {
            if (Beers.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No beer with id {id}", id));
            var current = Beers.First(c => c.Id == id);
            current.Name = beer.Name;
            current.Brewery = _context.Breweries.First(b => b.Id == beer.Brewery.Id);
            current.Description = beer.Description;
            current.Image = beer.Image;
            await _context.SaveChangesAsync();
            return current;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<Beer> Delete(string id)
        {
            if (Beers.All(c => c.Id != id))
                throw new KeyNotFoundException($"No record with id {id}");
            var current = Beers.First(c => c.Id == id);
            _context.Beers.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
