using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ontap.Auth;
using Ontap.Models;
using Ontap.Util;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/prices")]
    [Authorize(Policy = "PubAdminUser")]
    public class BeerPricesController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BeerPricesController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u => u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        private IEnumerable<BeerPrice> BeerPrices => _context.BeerPrices.Include(b => b.Pub).Include(b => b.Beer);

        private IEnumerable<BeerPrice> CurrentBeerPrices => _context.BeerPrices.Where(b => b.ValidTo == null || b.ValidTo < DateTime.UtcNow).Include(b => b.Pub).Include(b => b.Beer);


        // GET: api/prices
        [HttpGet]
        public IEnumerable<BeerPrice> Get() => CurrentBeerPrices.ToArray();

        // POST api/prices
        [HttpPost]
        public async Task<BeerPrice> Post([FromBody] BeerPrice price)
        {
            price.Beer = _context.Beers.First(b => b.Id == price.Beer.Id);
            price.Pub = _context.Pubs.First(b => b.Id == price.Pub.Id);
            if (!(await GetUser()).HasRights(price.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            price.Updated = DateTime.UtcNow;
            _context.BeerPrices.Add(price);
            await _context.SaveChangesAsync();
            return price;
        }

        // PUT api/BeerPrices/12
        [HttpPut("{id}")]
        public async Task<BeerPrice> Put(int id, [FromBody]BeerPrice price)
        {
            if (BeerPrices.All(c => c.Id != id))
                throw new KeyNotFoundException($"No beer price with id {id}");
            var current = _context.BeerPrices.First(c => c.Id == id);
            current.Beer = _context.Beers.First(b => b.Id == price.Beer.Id);
            current.Pub = _context.Pubs.First(b => b.Id == price.Pub.Id);
            if (!(await GetUser()).HasRights(current.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            current.Updated = DateTime.UtcNow;
            current.ValidFrom = price.ValidFrom;
            current.ValidTo = price.ValidTo;
            current.Price = price.Price;
            current.Volume = price.Volume;

            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/prices/5
        [HttpDelete("{id}")]
        public async Task<BeerPrice> Delete(int id)
        {
            if (BeerPrices.All(c => c.Id != id))
                throw new KeyNotFoundException($"No tap with id {id}");
            var current = _context.BeerPrices.First(c => c.Id == id);
            var pub = _context.Pubs.First(b => b.Id == current.Pub.Id);
            if (!(await GetUser()).HasRights(pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            current.ValidTo = DateTime.UtcNow; //Instead of removal, marking as obsolete
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
