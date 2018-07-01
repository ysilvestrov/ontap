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
    [Route("api/taps")]
    [Authorize(Policy = "PubAdminUser")]
    public class TapsController : Controller
    {
        private readonly DataContext _context;

        public TapsController(DataContext context)
        {
            _context = context;
  
        }

        private IEnumerable<Tap> Taps => _context.Taps.Include(b => b.Pub).OrderBy(t => t.Pub.Name).ThenBy(t=>t.Number);


        // GET: api/taps
        [HttpGet]
        public IEnumerable<Tap> Get() => Taps.ToArray();

        // POST api/taps
        [HttpPost]
        public async Task<Tap> Post([FromBody] Tap tap)
        {
            _context.Taps.Add(tap);
            await _context.SaveChangesAsync();
            return tap;
        }

        // PUT api/Taps/12
        [HttpPut("{id}")]
        public async Task<Tap> Put(int id, [FromBody]Tap tap)
        {
            if (Taps.All(c => c.Id != id))
                throw new KeyNotFoundException($"No tap with id {id}");
            var current = Taps.First(c => c.Id == id);
            current.Fitting = tap.Fitting;
            current.HasHopinator = current.HasHopinator;
            current.NitrogenPercentage = tap.NitrogenPercentage;
            current.Number = tap.Number;
            current.Status = tap.Status;
            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/taps/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<Tap> Delete(int id)
        {
            if (Taps.All(c => c.Id != id))
                throw new KeyNotFoundException($"No tap with id {id}");
            var current = _context.Taps.First(c => c.Id == id);
            _context.Taps.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
