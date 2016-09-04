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
    public class BeersController : Controller
    {
        private readonly DataContext _context;

        public BeersController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<Beer> Beers => _context.Beers;


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
                throw new ArgumentException(string.Format("Beer with id {id} already exists", beer.Id));
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
            current.Brewery = beer.Brewery;
            current.Description = beer.Description;
            await _context.SaveChangesAsync();
            return current;
        }

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
