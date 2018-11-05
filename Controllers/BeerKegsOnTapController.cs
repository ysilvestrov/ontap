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
    [Route("api/beerkegsontap")]
    public class BeerKegsOnTapController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BeerKegsOnTapController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u => u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        private IEnumerable<BeerKegOnTap> BeerKegsOnTap => _context.BeerKegsOnTap
            .Include(bk => bk.Tap).ThenInclude(t => t.Pub)
            .Include(bk => bk.Keg).ThenInclude(k => k.Keg)
            .Include(bk => bk.Keg).ThenInclude(k => k.Beer).ThenInclude(b=>b.Brewery)
            .Include(bk => bk.Keg).ThenInclude(k => k.Buyer);


        // GET: api/beerkegsontap
        [HttpGet]
        public IEnumerable<BeerKegOnTap> Get()
        {
            var beerKegOnTaps = BeerKegsOnTap.ToArray();
            foreach (var bk in beerKegOnTaps)
            {
                if (bk.Tap != null)
                {
                    bk.Tap.Pub.BeerKegsBought = null;
                    bk.Tap.Pub.Taps = null;
                    bk.Tap.Pub.BeerPrices = null;
                }
                if (bk.Keg != null)
                {
                    bk.Keg.BeerKegsOnTap = null;
                    bk.Keg.Beer.BeerKegs = null;
                    bk.Keg.Beer.BeerPrices = null;
                    bk.Keg.Beer.Substitutions = null;
                    bk.Keg.Beer.Brewery.Admins = null;
                    bk.Keg.Beer.Brewery.BeerKegsOwned = null;
                    bk.Keg.Beer.Brewery.Beers = null;
                    bk.Keg.Beer.Brewery.Substitutions = null;
                    bk.Keg.Buyer.BeerKegsBought = null;
                    bk.Keg.Buyer.BeerPrices = null;
                    bk.Keg.Buyer.Taps = null;
                    if (bk.Keg.Owner != null)
                    {
                        bk.Keg.Owner.Admins = null;
                        bk.Keg.Owner.BeerKegsOwned = null;
                        bk.Keg.Owner.Beers = null;
                        bk.Keg.Owner.Substitutions = null;
                    }
                }
            }

            return beerKegOnTaps;
        }

        // GET: api/beerkegsontap/yourpub/true
        [HttpGet("{pubId},{pure}")]
        public IEnumerable<BeerKegOnTap> Get(string pubId, bool pure)
        {
            var beerKegOnTaps = (pure ? _context.BeerKegsOnTap : BeerKegsOnTap).Where(bk => bk.Tap.Pub.Id == pubId);
            return beerKegOnTaps.ToArray();
        }

        // POST api/beerkegsontap
        [HttpPost]
        [Authorize(Policy = "BreweryOrPubAdminUser")]
        public async Task<BeerKegOnTap> Post([FromBody] BeerKegOnTap keg)
        {
            await CheckUserRights(keg);
            if (keg.Tap == null || keg.Keg == null)
                throw new ArgumentNullException(nameof(keg), "Cannot create beerkeg on tap entry without beerkeg and tap.");
            keg.Tap = _context.Taps.FirstOrDefault(t => t.Id == keg.Tap.Id);
            keg.Keg = _context.BeerKegs.FirstOrDefault(k => k.Id == keg.Keg.Id);
            _context.BeerKegsOnTap.Add(keg);      
            await _context.SaveChangesAsync();
            return keg;
        }

        private async Task CheckUserRights(BeerKegOnTap keg)
        {
            var user = await GetUser();
            if (
                //if any buyer exists, it should be pub user have access to
                (keg?.Keg?.Buyer == null || !user.HasRights(keg.Keg.Buyer)) &&
                keg?.Tap?.Pub != null && !user.HasRights(keg.Tap.Pub)
            )
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
        }

        // PUT api/beerkegsontap/12
        [HttpPut("{id}")]
        [Authorize(Policy = "BreweryOrPubAdminUser")]
        public async Task<BeerKegOnTap> Put(int id, [FromBody]BeerKegOnTap keg)
        {
            if (BeerKegsOnTap.All(c => c.Id != id))
                throw new KeyNotFoundException($"No beer keg with id {id}");
            if (keg.Keg == null)
                throw new ArgumentNullException(nameof(keg), "Cannot update beerkeg on tap entry without beerkeg.");

            var current = BeerKegsOnTap.First(c => c.Id == id);
            await CheckUserRights(current);

            current.DeinstallTime = keg.DeinstallTime;
            current.InstallTime = keg.InstallTime;
            current.Priority = keg.Priority;
            current.Tap = keg.Tap != null ? _context.Taps.FirstOrDefault(b => b.Id == keg.Tap.Id) : null;
            current.Keg = _context.BeerKegs.FirstOrDefault(k => k.Id == keg.Keg.Id);

            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/beerkegsontap/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BreweryOrPubAdminUser")]
        public async Task<BeerKegOnTap> Delete(int id)
        {
            if (BeerKegsOnTap.All(c => c.Id != id))
                throw new KeyNotFoundException($"No keg with id {id}");
            var current = _context.BeerKegsOnTap.First(c => c.Id == id);
            //_context.BeerKegsOnTap.Remove(current);
            //Deinstall the keg istead
            current.DeinstallTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
