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
    [Route("api/kegs")]
    [Authorize(Policy = "BreweryOrPubAdminUser")]
    public class KegsController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KegsController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u => u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        private IEnumerable<Keg> BeerKegs => _context.Kegs
            .Include(k => k.BeerKegs).ThenInclude(bk => bk.Buyer)
            .Include(k => k.BeerKegs).ThenInclude(bk => bk.Owner);


        // GET: api/kegs
        [HttpGet]
        public IEnumerable<Keg> Get()
        {
            var kegs = BeerKegs.ToArray();
            foreach (var keg in kegs)
            {
                foreach (var bk in keg.BeerKegs)
                {
                    bk.BeerKegsOnTap = null;
                    if (bk.Buyer != null)
                    {
                        bk.Buyer.BeerKegsBought = null;
                        bk.Buyer.BeerPrices = null;
                        bk.Buyer.Taps = null;
                        bk.Buyer.Admins = null;
                    }
                    if (bk.Owner != null)
                    {
                        bk.Owner.Admins = null;
                        bk.Owner.BeerKegsOwned = null;
                        bk.Owner.Substitutions = null;
                    }
                }
            }
            return kegs;
        }

        // POST api/kegs
        [HttpPost]
        public async Task<Keg> Post([FromBody] Keg keg)
        {
            await CheckUserRights(keg);
            _context.Kegs.Add(keg);      
            await _context.SaveChangesAsync();
            return keg;
        }

        private async Task CheckUserRights(Keg keg)
        {
            var beerKeg = keg.BeerKegs?.Where(bk => bk.Status != KegStatus.Inactive).OrderBy(bk => bk.PackageDate)
                .LastOrDefault();
            var user = await GetUser();
            if (
                beerKeg?.Buyer != null && !user.HasRights(beerKeg.Buyer) ||
                beerKeg?.Owner != null && !user.HasRights(beerKeg.Owner)
            )
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
        }

        // PUT api/kegs/12
        [HttpPut("{id}")]
        public async Task<Keg> Put(int id, [FromBody]Keg keg)
        {
            if (BeerKegs.All(c => c.Id != id))
                throw new KeyNotFoundException($"No keg with id {id}");
            var current = BeerKegs.First(c => c.Id == id);
            await CheckUserRights(current);

            current.Fitting = keg.Fitting;
            current.EmptyWeight = keg.EmptyWeight;
            current.ExternalId = keg.ExternalId;
            current.Material = keg.Material;
            current.EmptyWeight = keg.EmptyWeight;
            current.IsReturnable = keg.IsReturnable;
            current.Volume = keg.Volume;

            await _context.SaveChangesAsync();
            return current;
        }

        // DELETE api/kegs/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<Keg> Delete(int id)
        {
            if (BeerKegs.All(c => c.Id != id))
                throw new KeyNotFoundException($"No keg with id {id}");
            var current = _context.Kegs.First(c => c.Id == id);
            _context.Kegs.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
