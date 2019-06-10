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
    [Route("api/taps")]
    [Authorize(Policy = "PubAdminUser")]
    public class TapsController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TapsController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u => u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        private IEnumerable<Tap> Taps => _context.Taps.Include(b => b.Pub).OrderBy(t => t.Pub.Name).ThenBy(t=>t.Number);


        // GET: api/taps
        [HttpGet]
        public IEnumerable<Tap> Get() => Taps.ToArray();

        // POST api/taps
        [HttpPost]
        public async Task<Tap> Post([FromBody] Tap tap)
        {          
            tap.Pub = _context.Pubs.First(b => b.Id == tap.Pub.Id);
            if (!(await GetUser()).HasRights(tap.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
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
            current.Pub = _context.Pubs.First(b => b.Id == tap.Pub.Id);
            if (!(await GetUser()).HasRights(current.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
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

        #region helpers
        private async Task<BeerKegOnTap[]> GetBeerKegsOnTap(Tap current)
        {
            var tap = await _context.Taps
                .Include(t => t.BeerKegsOnTap)
                .ThenInclude(bk => bk.Keg)
                .ThenInclude(k => k.Beer)
                .ThenInclude(b => b.Brewery)
                .Include(t => t.BeerKegsOnTap)
                .ThenInclude(bk => bk.Keg)
                .ThenInclude(k => k.Weights)
                .Include(t => t.BeerKegsOnTap)
                .ThenInclude(bk => bk.Keg)
                .ThenInclude(k => k.Keg)
                .FirstAsync(t => t.Id == current.Id);
            return tap.BeerKegsOnTap.Where(bk => bk.DeinstallTime == null || bk.DeinstallTime > DateTime.UtcNow).ToArray();
        }
        #endregion

        #region api/taps/{tap}/beer

        // DELETE api/taps/{tap}/beer
        [HttpDelete("{id}/beer")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKegOnTap[]> RemoveBeerFromTap(int id)
        {
            var current = await _context.Taps
                .Include(t => t.Pub)
                .Include(t => t.BeerKegsOnTap)
                    .ThenInclude(bk => bk.Keg)
                    .ThenInclude(k=>k.Weights)
                .Include(t => t.BeerKegsOnTap)
                    .ThenInclude(bk => bk.Keg)
                    .ThenInclude(k=>k.Keg)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (current == null)
                throw new KeyNotFoundException($"No tap with id {id}");
            if (!(await GetUser()).HasRights(current.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            var kegsOnTap = current.BeerKegsOnTap
                .Where(bk => bk.DeinstallTime == null || bk.DeinstallTime > DateTime.UtcNow).ToArray();
            if (!kegsOnTap.Any())
            {
                throw new KeyNotFoundException("No beer on tap to remove");
            }
            foreach (var beerKegOnTap in current.BeerKegsOnTap)
            {
                if (kegsOnTap.All(k => beerKegOnTap.Id != k.Id)) continue;
                beerKegOnTap.DeinstallTime = DateTime.UtcNow;
                beerKegOnTap.Keg.DeinstallationDate = DateTime.UtcNow;
                if (!beerKegOnTap.Keg.Weights.Any() || beerKegOnTap.Keg.Weights.OrderBy(w => w.Date).Last().Weight >
                    beerKegOnTap.Keg.Keg.EmptyWeight + 0.5m)
                {
                    beerKegOnTap.InstallTime = null;
                    beerKegOnTap.Keg.InstallationDate = null;
                }
            }

            await _context.SaveChangesAsync();
            return await GetBeerKegsOnTap(current);
        }

        // DELETE api/taps/{tap}/beer/{keg}
        [HttpDelete("{id}/beer/{kegId}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKegOnTap[]> RemoveBeerFromTapDirectQueue(int id, int kegId)
        {
            var current = await _context.Taps
                .Include(t => t.Pub)
                .Include(t => t.BeerKegsOnTap)
                    .ThenInclude(bk => bk.Keg)
                    .ThenInclude(k=>k.Weights)
                .Include(t => t.BeerKegsOnTap)
                    .ThenInclude(bk => bk.Keg)
                    .ThenInclude(k=>k.Keg)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (current == null)
                throw new KeyNotFoundException($"No tap with id {id}");
            if (!(await GetUser()).HasRights(current.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }

            var keg = current.BeerKegsOnTap.OrderBy(bk => bk.Priority)
                .First(bk =>  bk.Id == kegId && bk.InstallTime == null || bk.InstallTime > DateTime.UtcNow);

            if (keg == null)
            {
                throw new KeyNotFoundException("No beer on tap to remove from queue");
            }

            keg.Tap = null;

            await _context.SaveChangesAsync();
            return await GetBeerKegsOnTap(current);
        }

        //PUT api/taps/{tap}/beer
        [HttpPut("{id}/beer")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKegOnTap[]> AddToDirectQueue(int id, [FromBody] BeerKegOnTap beerKegOnTap)
        {
            var current = await _context.Taps
                .Include(t => t.Pub)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (current == null)
                throw new KeyNotFoundException($"No tap with id {id}");
            if (!(await GetUser()).HasRights(current.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            var bko = await _context.BeerKegsOnTap.Include(bk => bk.Tap).FirstOrDefaultAsync(bk => bk.Id == beerKegOnTap.Id);
            if (bko?.Tap != null)
            {
                throw new AlreadyExistsException("The beer is already on tap or in the direct queue");
            }

            var keg = await _context.BeerKegsOnTap
                .Include(bk => bk.Keg)
                .FirstOrDefaultAsync(bk => beerKegOnTap.Id == bk.Id);
            if (keg == null)
                throw new KeyNotFoundException($"No beer keg on tap with id {beerKegOnTap.Id}");

            var priority = 0;

            var bkos = _context.BeerKegsOnTap
                               .Include(bk => bk.Tap)
                               .Where(bk =>
                                    bk.Tap.Id == id && 
                                   !(bk.InstallTime != null && bk.InstallTime < DateTime.UtcNow &&
                                     (bk.DeinstallTime == null || bk.DeinstallTime > DateTime.UtcNow))).ToArray();
            if (bkos.Any())
                priority = bkos.Max(bk => bk.Priority) + 1;

            keg.Tap = current;
            keg.InstallTime = null;
            keg.Keg.InstallationDate = null;
            keg.DeinstallTime = null;
            keg.Keg.DeinstallationDate = null;
            keg.Priority = priority;

            await _context.SaveChangesAsync();
            return await GetBeerKegsOnTap(current);
        }

        //POST api/taps/{tap}/beer
        [HttpPost("{id}/beer")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKegOnTap[]> PutOnTapFromDirectQueue(int id)
        {
            var current = await _context.Taps
                .Include(t => t.Pub)
                .Include(t => t.BeerKegsOnTap)
                .ThenInclude(bk => bk.Keg)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (current == null)
                throw new KeyNotFoundException($"No tap with id {id}");
            if (!(await GetUser()).HasRights(current.Pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            if (current.BeerKegsOnTap.Any(bk => bk.InstallTime != null && bk.InstallTime < DateTime.UtcNow && (bk.DeinstallTime == null || bk.DeinstallTime > DateTime.UtcNow)))
            {
                throw new AlreadyExistsException("There is a beer already on tap. Please remove it first.");
            }

            var keg = current.BeerKegsOnTap.OrderBy(bk => bk.Priority)
                .First(bk => bk.InstallTime == null || bk.InstallTime > DateTime.UtcNow);

            keg.InstallTime = DateTime.UtcNow;
            keg.Keg.InstallationDate = DateTime.UtcNow;
            keg.DeinstallTime = null;
            keg.Keg.DeinstallationDate = null;
            var priority = keg.Priority;
            keg.Priority = 0;

            if (priority > 0)
            {
                foreach (var beerKegOnTap in current.BeerKegsOnTap
                    .Where(bk =>
                        !(bk.InstallTime != null && bk.InstallTime < DateTime.UtcNow &&
                          (bk.DeinstallTime == null || bk.DeinstallTime > DateTime.UtcNow))))
                {
                    beerKegOnTap.Priority--;
                }
            }

            await _context.SaveChangesAsync();
            return await GetBeerKegsOnTap(current);
        }

        #endregion
        }
}
