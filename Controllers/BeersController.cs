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

        // POST api/beers
        [HttpPost]
        public async Task<Beer> Post([FromBody] Beer beer)
        {
            var pid = beer.Name.MakeId();
            var id = pid;

            for (var i = 0; Beers.Any(c => c.Id == id); id = $"{pid}_{i}", i++) ;

            beer.Id = id;
            beer.Brewery = _context.Breweries.First(b => b.Id == beer.Brewery.Id);
            _context.Beers.Add(beer);
            await _context.SaveChangesAsync();
            return beer;
        }

        // PUT api/beers/VarvarPilsner
        [HttpPut("{id}")]
        public async Task<Beer> Put(string id, [FromBody]Beer beer)
        {
            if (Beers.All(c => c.Id != id))
                throw new KeyNotFoundException($"No beer with id {id}");
            var current = Beers.First(c => c.Id == id);
            current.Name = beer.Name;
            current.Brewery = _context.Breweries.First(b => b.Id == beer.Brewery.Id);
            current.Description = beer.Description;
            current.Image = beer.Image;
            current.Alcohol = beer.Alcohol;
            current.Gravity = beer.Gravity;
            current.Ibu = beer.Ibu;
            current.BjcpStyle = beer.BjcpStyle;
            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/beers/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<Beer> Delete(string id, [FromQuery] string replacementId)
        {
            if (Beers.All(c => c.Id != id))
                throw new KeyNotFoundException($"No beer with id {id}");
            var current = _context.Beers.First(c => c.Id == id);
            if (!string.IsNullOrWhiteSpace(replacementId) && Beers.Any(c => c.Id == replacementId))
            {
                var replacement = _context.Beers.First(b => b.Id == replacementId);
                _context.BeerSubstitutions.Add(new BeerSubstitution { Beer = replacement, Name = current.Name });
                var kegs = _context.BeerKegs.Where(b => b.Beer.Id == id);
                foreach (var keg in kegs)
                {
                    keg.Beer = replacement;
                }
                var prices = _context.BeerPrices.Where(b => b.Beer.Id == id);
                foreach (var price in prices)
                {
                    price.Beer = replacement;
                }
            }
            else if (_context.BeerKegs.Any(b => b.Beer.Id == id) || _context.BeerPrices.Any(b => b.Beer.Id == id))
                throw new AlreadyExistsException("Cannot delete brewery with serves");
            _context.Beers.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
