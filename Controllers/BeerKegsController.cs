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
using Ontap.Util;
using Ontap.Auth;
using Ontap.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/beerkegs")]
    [Authorize(Policy = "BreweryOrPubAdminUser")]
    public class BeerKegsController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BeerKegsController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u => u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        private IEnumerable<BeerKeg> BeerKegs => _context.BeerKegs.Include(bk => bk.Beer).Include(bk => bk.Keg).Include(bk => bk.Owner).Include(bk => bk.Buyer);


        // GET: api/kegs
        [HttpGet]
        public IEnumerable<BeerKeg> Get() => BeerKegs.ToArray();

        // POST api/kegs
        [HttpPost]
        public async Task<BeerKeg> Post([FromBody] BeerKeg keg)
        {
            await CheckUserRights(keg);
            if (keg.Beer == null || keg.Keg == null)
                throw new ArgumentNullException(nameof(keg), "Cannot create beerkeg without beer and keg.");
            keg.Beer = _context.Beers.FirstOrDefault(b => b.Id == keg.Beer.Id);
            keg.Keg = _context.Kegs.FirstOrDefault(k => k.Id == keg.Keg.Id);
            if (keg.Buyer != null)
                keg.Buyer = _context.Pubs.FirstOrDefault(p => p.Id == keg.Buyer.Id);
            if (keg.Owner != null)
                keg.Owner = _context.Breweries.FirstOrDefault(b => b.Id == keg.Owner.Id);
            _context.BeerKegs.Add(keg);      
            await _context.SaveChangesAsync();
            return keg;
        }

        private async Task CheckUserRights(BeerKeg keg)
        {
            var user = await GetUser();
            if (
                keg?.Buyer != null && !user.HasRights(keg.Buyer) ||
                keg?.Owner != null && !user.HasRights(keg.Owner)
            )
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
        }

        // PUT api/kegs/12
        [HttpPut("{id}")]
        public async Task<BeerKeg> Put(int id, [FromBody]BeerKeg keg)
        {
            if (BeerKegs.All(c => c.Id != id))
                throw new KeyNotFoundException($"No beer keg with id {id}");
            if (keg.Beer == null || keg.Keg == null)
                throw new ArgumentNullException(nameof(keg), "Cannot update beerkeg without beer and keg.");

            var current = BeerKegs.First(c => c.Id == id);
            await CheckUserRights(current);

            current.PackageDate = keg.PackageDate;
            current.Status = keg.Status;
            current.ArrivalDate = keg.ArrivalDate;
            current.BestBeforeDate = keg.BestBeforeDate;
            current.BrewingDate = keg.BrewingDate;
            current.InstallationDate = keg.InstallationDate;
            current.DeinstallationDate = keg.DeinstallationDate;
            current.Beer = _context.Beers.FirstOrDefault(b => b.Id == keg.Beer.Id);
            current.Keg = _context.Kegs.FirstOrDefault(k => k.Id == keg.Keg.Id);
            if (keg.Buyer != null)
                current.Buyer = _context.Pubs.FirstOrDefault(p => p.Id == keg.Buyer.Id);
            if (keg.Owner != null)
                current.Owner = _context.Breweries.FirstOrDefault(b => b.Id == keg.Owner.Id);

            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/kegs/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<BeerKeg> Delete(int id)
        {
            if (BeerKegs.All(c => c.Id != id))
                throw new KeyNotFoundException($"No keg with id {id}");
            var current = _context.BeerKegs.First(c => c.Id == id);
            _context.BeerKegs.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
