using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
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
    [Route("api/[controller]")]
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

        public IEnumerable<Pub> Pubs => _context.Pubs
            .Include(p => p.City)
            .Include(p => p.BeerServedInPubs)
            .ThenInclude(s => s.Served)
            .ThenInclude(b => b.Brewery)
            .ThenInclude(b=>b.Country)
            .ToArray();


        // GET: api/pubs
        [HttpGet]
        public IEnumerable<Pub> Get()
        {
            var pubs = Pubs;
            var enumerable = pubs as Pub[] ?? pubs.ToArray();
            foreach (var pub in enumerable)
            {
                pub.BeerServedInPubs.ToArray();
            }
            return enumerable;
        }

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
            pub.Id = Utilities.CreateId(pub.Name, i => Pubs.Any(c => c.Id == i));
            _context.Pubs.Add(pub);
            await _context.SaveChangesAsync();
            return await _context.Pubs.FirstAsync(p => p.Id == pub.Id);
        }

        // POST api/pubs
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
            var options = JsonConvert.DeserializeObject<Dictionary<string, object>>(pub.ParserOptions);
            var parser = new GoogleSheetParser();
            var serves = parser
                .Parse(pub, _context.Beers, _context.Breweries, options,
                    _context.Countries.First(c => c.Id == "UA"))
                .Where(s => s.Served?.Brewery != null).ToArray();
            var beers = serves.Select(s => s.Served).Where(b => _context.Beers.All(_ => _.Id != b.Id));
            _context.Beers.AddRange(beers);
            _context.Breweries.AddRange(
                serves.Select(s => s.Served.Brewery).Where(b => _context.Breweries.All(_ => _.Id != b.Id)));
            _context.BeerServedInPubs.RemoveRange(_context.BeerServedInPubs.Where(s => s.ServedIn.Id == id));
            foreach (var serve in serves)
            {
                serve.Served = _context.Beers.FirstOrDefault(_ => _.Id == serve.Served.Id) ?? serve.Served;
            }
            _context.BeerServedInPubs.AddRange(serves);
            await _context.SaveChangesAsync();
            var result = string.Join("\r\n",
                serves.Select(s => $"{s.Tap}: {s.Served.Brewery.Name} - {s.Served.Name}, {s.Volume}l for {s.Price} UAH"));
            return result;
        }

        // PUT api/cities/Kharkiv
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
            current.BookingUrl = pub.BookingUrl;
            current.FacebookUrl = pub.FacebookUrl;
            current.VkontakteUrl = pub.VkontakteUrl;
            current.WebsiteUrl = pub.WebsiteUrl;
            current.ParserOptions = pub.ParserOptions;
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
