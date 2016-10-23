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
    public class PubsController : Controller
    {
        private readonly DataContext _context;

        public PubsController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<Pub> Pubs => _context.Pubs
            .Include(p => p.City)
            .Include(p => p.BeerServedInPubs)
            .ThenInclude(s => s.Served)
            .ThenInclude(b => b.Brewery)
            .ToArray();


        // GET: api/pubs
        [HttpGet]
        public IEnumerable<Pub> Get()
        {
            var pubs = Pubs;
            var enumerable = pubs as Pub[] ?? pubs.ToArray();
            foreach (var pub in enumerable)
            {
                pub.BeerServedInPubs.ToArray();
            }
            return enumerable;
        }

        // POST api/values
        [HttpPost]
        public async Task<Pub> Post([FromBody] Pub pub)
        {
            if (Pubs.Any(c => c.Id == pub.Id))
                throw new ArgumentException(string.Format("Pub with id {id} already exists", pub.Id));
            _context.Pubs.Add(pub);
            await _context.SaveChangesAsync();
            return pub;
        }

        // PUT api/cities/Kharkiv
        [HttpPut("{id}")]
        public async Task<Pub> Put(string id, [FromBody]Pub pub)
        { 
            if (Pubs.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No pub with id {id}", id));
            var current = Pubs.First(c => c.Id == id);
            current.Name = pub.Name;
            current.City = _context.Cities.First(p => p.Id == pub.City.Id);
            current.Address = pub.Address;
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
