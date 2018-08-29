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
    [Route("api/beerkegweights")]
    [Authorize(Policy = "BreweryOrPubAdminUser")]
    public class BeerKegWeightsController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BeerKegWeightsController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u => u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        private IEnumerable<BeerKegWeight> BeerKegWeights => _context.BeerKegWeights
            .Include(bk => bk.Keg).ThenInclude(k => k.Keg)
            .Include(bk => bk.Keg).ThenInclude(k => k.Beer).ThenInclude(b=>b.Brewery)
            .Include(bk => bk.Keg).ThenInclude(k => k.Buyer);


        // GET: api/beerkegsontap
        [HttpGet]
        public IEnumerable<BeerKegWeight> Get() => BeerKegWeights.ToArray();

        // POST api/beerkegsontap
        [HttpPost]
        public async Task<BeerKegWeight> Post([FromBody] BeerKegWeight weight)
        {
            await CheckUserRights(weight);
            if (weight.Keg == null)
                throw new ArgumentNullException(nameof(weight), "Cannot weight a beerkeg entry without beerkeg.");
            weight.Keg = _context.BeerKegs.Include(k => k.Keg).Include(k => k.Beer).ThenInclude(b => b.Brewery).FirstOrDefault(k => k.Id == weight.Keg.Id);
            _context.BeerKegWeights.Add(weight);      
            await _context.SaveChangesAsync();
            return weight;
        }

        private async Task CheckUserRights(BeerKegWeight weight)
        {
            var user = await GetUser();
            if (
                //if any buyer exists, it should be pub user have access to
                weight?.Keg?.Buyer == null || !user.HasRights(weight.Keg.Buyer)
                
            )
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
        }

        // PUT api/beerkegsontap/12
        [HttpPut("{id}")]
        public async Task<BeerKegWeight> Put(int id, [FromBody]BeerKegWeight weight)
        {
            if (BeerKegWeights.All(c => c.Id != id))
                throw new KeyNotFoundException($"No beer weight with id {id}");
            if (weight.Keg == null)
                throw new ArgumentNullException(nameof(weight), "Cannot update weight entry without beerkeg.");

            var current = BeerKegWeights.First(c => c.Id == id);
            await CheckUserRights(current);

            current.Date = weight.Date;
            current.Weight = weight.Weight;
            current.Keg = _context.BeerKegs.FirstOrDefault(k => k.Id == weight.Keg.Id);

            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/beerkegsontap/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<BeerKegWeight> Delete(int id)
        {
            if (BeerKegWeights.All(c => c.Id != id))
                throw new KeyNotFoundException($"No weight with id {id}");
            var current = _context.BeerKegWeights.First(c => c.Id == id);
            _context.BeerKegWeights.Remove(current);
            //Deinstall the weight istead
            //current.DeinstallTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
