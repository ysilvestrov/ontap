using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ontap.Auth;
using Ontap.Models;
using Ontap.Util;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/pubs")]
    public class PubsController : Controller
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u=>u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        public PubsController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private IEnumerable<Pub> Pubs => _context.Pubs
            .Include(p => p.City)
            .Include(p => p.BeerPrices)
            .ThenInclude(s => s.Beer)
            .ThenInclude(b => b.Brewery)
            .ThenInclude(b => b.Country)
            .OrderBy(p => p.Name)
            .ToArray();


        // GET: api/pubs
        [HttpGet]
        public IEnumerable<Pub> Get() => Pubs;

        // GET: api/pubs/id
        [HttpGet("{id}")]
        public Pub Get(string id) => Pubs.First(p => p.Id == id);

        // POST api/pubs
        /// <summary>
        /// Creates new pub
        /// </summary>
        /// <param name="pub">Pub to create</param>
        /// <returns>Processed pub entity</returns>
        [HttpPost]
        [Authorize(Policy = "AdminUser")]
        public async Task<Pub> Post([FromBody] Pub pub)
        {
            var current = pub;
            current.Id = Utilities.CreateId(pub.Name, i => Pubs.Any(c => c.Id == i));
            current.City = _context.Cities.First(c => c.Id == pub.City.Id);
            _context.Pubs.Add(current);
            await _context.SaveChangesAsync();
            return await _context.Pubs.FirstAsync(p => p.Id == pub.Id);
        }

        // PATCH api/pubs/id
        /// <summary>
        /// Import pub's beers
        /// </summary>
        /// <param name="id">Pub to run import</param>
        /// <returns>Import log</returns>
        [HttpPatch("{id}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<string> Run(string id)
        {
            var pub = Pubs.First(c => c.Id == id);
            if (!(await GetUser()).HasRights(pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            var result = new StringBuilder();
            ParsePubData(pub, result);
            await _context.SaveChangesAsync();
            return result.ToString();
        }

        private void ParsePubData(Pub pub, StringBuilder sb)
        {
            var options = JsonConvert.DeserializeObject<Dictionary<string, object>>(pub.ParserOptions);
            var parser = new GoogleSheetParser();
            var serves = parser
                .Parse(pub, _context.Beers, _context.Breweries, options,
                    _context.Countries.First(c => c.Id == "UA"), force: true,
                    substitutions: _context.BrewerySubstitutions.ToDictionary(s => s.Name, s => s.Brewery))
                .Where(s => s.Beer?.Brewery != null).ToArray();
            if (serves.Length > 0)
            {
                //save all the beers
                var beers = serves.Select(s => s.Beer).Where(b => _context.Beers.All(_ => _.Id != b.Id));
                _context.Beers.AddRange(beers);
                //save all the breweries
                _context.Breweries.AddRange(
                    serves.Select(s => s.Beer.Brewery).Where(b => _context.Breweries.All(_ => _.Id != b.Id)));
                //update beer price and kegs
                //remove discontinued beers
                var discontinuedBeerPrices = _context.BeerPrices.Where(bp => bp.Pub.Id == pub.Id && bp.ValidTo == null)
                    .Where(bp => serves.All(s => s.Beer.Id != bp.Beer.Id));
                foreach (var beer in discontinuedBeerPrices)
                {
                    _context.BeerPrices.First(bp => bp.Id == beer.Id).ValidTo = DateTime.UtcNow;
                }
                var discontinuedBeerKegs = _context.BeerKegsOnTap.Where(bk => bk.Tap.Pub.Id == pub.Id && bk.DeinstallTime == null)
                    .Where(bk => serves.All(s => s.Beer.Id != bk.Keg.Beer.Id));
                foreach (var beer in discontinuedBeerKegs)
                {
                    _context.BeerKegsOnTap.First(bp => bp.Id == beer.Id).DeinstallTime = DateTime.UtcNow;
                }
                //update beers
                foreach (var serve in serves)
                {
                    serve.Beer = _context.Beers.FirstOrDefault(_ => _.Id == serve.Beer.Id) ?? serve.Beer;
                }
                //add new prices
                var newBeerPrices = serves
                    .Where(s => !_context.BeerPrices.Any(bp =>
                        bp.Pub.Id == pub.Id && bp.ValidTo == null && bp.Beer.Id == s.Beer.Id)).Select(s => new BeerPrice
                    {
                        Beer = s.Beer,
                        Pub = pub,
                        Price = s.Price,
                        Updated = DateTime.UtcNow,
                        ValidFrom = DateTime.UtcNow,
                        Volume = s.Volume
                    });
                _context.BeerPrices.AddRange(newBeerPrices);
                //add new kegs
                var newBeerServes = serves
                    .Where(s => !_context.BeerKegsOnTap.Any(bp =>
                        bp.Tap.Pub.Id == pub.Id && bp.DeinstallTime == null && bp.Keg.Beer.Id == s.Beer.Id)).ToArray();
                foreach (var serve in newBeerServes)
                {
                    var keg = new Keg
                    {
                        Volume = 30,
                        EmptyWeight = 5,
                        ExternalId = $"{serve.Pub.Id}_{serve.Beer.Id}_{DateTime.UtcNow.Ticks}",
                        IsReturnable = false
                    };
                    var beerKeg = new BeerKeg
                    {
                        ArrivalDate = DateTime.UtcNow,
                        PackageDate = DateTime.UtcNow.AddDays(-1),
                        Beer = serve.Beer,
                        Buyer = serve.Pub,
                        Keg = keg,
                        InstallationDate = DateTime.UtcNow
                    };
                    var tap = _context.Taps.FirstOrDefault(t => t.Pub.Id == serve.Pub.Id && t.Number == serve.Tap.ToString());
                    if (tap == null)
                    {
                        tap = new Tap {Pub = serve.Pub, Number = serve.Tap.ToString()};
                        _context.Taps.Add(tap);
                    }
                    var kegOnTap = new BeerKegOnTap {InstallTime = DateTime.UtcNow, Keg = beerKeg, Tap = tap};
                    _context.Kegs.Add(keg);
                    _context.BeerKegs.Add(beerKeg);
                    _context.BeerKegsOnTap.Add(kegOnTap);
                }
                sb.AppendLine(string.Join("\r\n",
                    serves.Select(
                        s => $"{s.Tap}: {s.Beer.Brewery.Name} - {s.Pub.Name}, {s.Volume}l for {s.Price} UAH")));
            }
        }

        // PATCH api/pubs
        /// <summary>
        /// Import all pub's beers
        /// </summary>
        /// <returns>Import log</returns>
        [HttpPatch]
        [Authorize(Policy = "AdminUser")]
        public async Task<string> Run()
        {
            var result = new StringBuilder();
            foreach (var pub in Pubs.Where(p => !string.IsNullOrWhiteSpace(p.ParserOptions)))
            {
                ParsePubData(pub, result);
            }
            await _context.SaveChangesAsync();
            return result.ToString();
        }

        // PUT api/pubs/BlackDoorPub
        [HttpPut("{id}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<Pub> Put(string id, [FromBody]Pub pub)
        { 
            if (Pubs.All(c => c.Id != id))
                throw new KeyNotFoundException($"No pub with id {id}");
            var current = Pubs.First(c => c.Id == id);
            if (!(await GetUser()).HasRights(pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            current.Name = pub.Name;
            current.City = _context.Cities.First(p => p.Id == pub.City.Id);
            current.Address = pub.Address;
            current.Image = pub.Image;
            current.TaplistHeaderImage = pub.TaplistHeaderImage;
            current.TaplistFooterImage = pub.TaplistFooterImage;
            current.BookingUrl = pub.BookingUrl;
            current.FacebookUrl = pub.FacebookUrl;
            current.VkontakteUrl = pub.VkontakteUrl;
            current.WebsiteUrl = pub.WebsiteUrl;
            current.ParserOptions = pub.ParserOptions;
            current.TapNumber = pub.TapNumber;
            await _context.SaveChangesAsync();
            return current;
        }

        // GET: api/pubs/{yourpub}/taps
        [HttpGet("{id}/taps")]
        public IEnumerable<Tap> GetTaps(string id, [FromQuery]bool pure = true)
        {
            var taps = _context.Taps.Include(t => t.Pub).Where(tap => tap.Pub.Id == id).ToArray();
            if (pure)
            {
                taps = taps.Select(t =>
                {
                    t.Pub = null;
                    return t;
                }).ToArray();
            }
            return taps;
        }
    }
}
