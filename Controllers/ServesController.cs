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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "BreweryOrPubAdminUser")]
    public class ServesController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.FirstAsync(u => u.Id == userId);
        }

        private bool HasUserRights(User user, BeerServedInPubs serve)
        {
            return user.IsAdmin || 
                (user.CanAdminBrewery && serve.Served.Brewery.Admins.Any(u=>u.User.Id == user.Id)) ||
                (user.CanAdminPub && serve.ServedIn.Admins.Any(u => u.User.Id == user.Id))
            ;
        }

        public ServesController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<BeerServedInPubs> Serves => _context.BeerServedInPubs
            .Include(s => s.Served)
            .ThenInclude(b => b.Brewery)
            .ThenInclude(b => b.Admins)
            .ThenInclude(a => a.User)
            .Include(s => s.ServedIn)
            .ThenInclude(p => p.Admins)
            .ThenInclude(a => a.User)
            .ToArray();


        // GET: api/serves
        [HttpGet]
        public IEnumerable<BeerServedInPubs> Get() => Serves as BeerServedInPubs[] ?? Serves.ToArray();

        // POST api/serves
        /// <exception cref="AlreadyExistsException">Record for the same beer and pub already exists</exception>
        [HttpPost]
        public async Task<BeerServedInPubs> Post([FromBody] BeerServedInPubs serve)
        {
            if (Serves.Any(s => s.Served.Id == serve.Served.Id && s.ServedIn.Id == serve.ServedIn.Id))
                throw new AlreadyExistsException("Record for the same beer and pub already exists");
            var current = serve;
            current.Served = _context.Beers.First(beer => beer.Id == serve.Served.Id);
            current.ServedIn = _context.Pubs.First(pub => pub.Id == serve.ServedIn.Id);
            if (!HasUserRights(await GetUser(), current))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
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
            if (!HasUserRights(await GetUser(), current))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            current.Served = _context.Beers.First(beer => beer.Id == serve.Served.Id);
            current.ServedIn = _context.Pubs.First(pub => pub.Id == serve.ServedIn.Id);
            current.Price = serve.Price;
            current.Tap = serve.Tap;
            current.Volume = serve.Volume;
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
            if (!HasUserRights(await GetUser(), current))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            _context.BeerServedInPubs.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
