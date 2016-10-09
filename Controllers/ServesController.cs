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
    public class ServesController : Controller
    {
        private readonly DataContext _context;

        public ServesController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<BeerServedInPubs> Serves => _context.BeerServedInPubs
            .Include(s => s.Served)
            .Include(s => s.ServedIn)
            .ToArray();


        // GET: api/serves
        [HttpGet]
        public IEnumerable<BeerServedInPubs> Get() => Serves as BeerServedInPubs[] ?? Serves.ToArray();

        // POST api/serves
        /// <exception cref="ArgumentException">Record for the same beer and pub already exists</exception>
        [HttpPost]
        public async Task<BeerServedInPubs> Post([FromBody] BeerServedInPubs serve)
        {
            if (Serves.Any(s => s.Served.Id == serve.Served.Id && s.ServedIn.Id == serve.ServedIn.Id))
                throw new ArgumentException("Record for the same beer and pub already exists");
            var current = serve;
            current.Served = _context.Beers.First(beer => beer.Id == serve.Served.Id);
            current.ServedIn = _context.Pubs.First(pub => pub.Id == serve.ServedIn.Id);
            _context.BeerServedInPubs.Add(current);
            await _context.SaveChangesAsync();
            return serve;
        }

        // PUT api/serves/123
        [HttpPut("{id}")]
        public async Task<BeerServedInPubs> Put(int id, [FromBody]BeerServedInPubs serve)
        { 
            if (Serves.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No record with id {id}", id));
            var current = Serves.First(c => c.Id == id);
            current.Served = _context.Beers.First(beer => beer.Id == serve.Served.Id);
            current.ServedIn = _context.Pubs.First(pub => pub.Id == serve.ServedIn.Id);
            current.Price = serve.Price;
            await _context.SaveChangesAsync();
            return current;
        }

        // Delete api/serves/123
        [HttpDelete("{id}")]
        public async Task<BeerServedInPubs> Delete(int id)
        { 
            if (Serves.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No record with id {id}", id));
            var current = Serves.First(c => c.Id == id);
            _context.BeerServedInPubs.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
