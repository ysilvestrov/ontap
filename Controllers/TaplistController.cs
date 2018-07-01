using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
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

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    public class TaplistController : Controller
    {
        public class TaplistItem
        {
            public string tapNumber;
            public string beerName;
            public string breweryName;
            public string beerColor;
            public decimal abv;
            public decimal bitterness;
            public decimal gravity;
            public Dictionary<string, decimal> prices;
        };

        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaplistController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<BeerServedInPubs> Serves => _context.BeerPrices
            .Where(s => s.ValidTo == null)
            .Include(s => s.Beer)
            .ThenInclude(b => b.Brewery)
            .ThenInclude(b => b.Admins)
            .ThenInclude(a => a.User)
            .Include(s => s.Pub)
            .ThenInclude(p => p.Admins)
            .ThenInclude(a => a.User)
            .Join(_context.BeerKegsOnTap.Where(bk => bk.DeinstallTime == null), price => new { beerId = price.Beer.Id, pubId = price.Pub.Id },
                tap => new { beerId = tap.Keg.Beer.Id, pubId = tap.Tap.Pub.Id }, (price, tap) => new BeerServedInPubs
                {
                    Beer = price.Beer,
                    Pub = price.Pub,
                    Price = price.Price,
                    Volume = price.Volume,
                    Updated = price.Updated,
                    Tap = tap.Tap.Number
                }).ToArray();

        // GET: api/taplist
        [HttpGet("{id}")]
        public IEnumerable<TaplistItem> Get(string id)
        {
            return Serves.Where(s => s.Pub.Id == id).OrderBy(s => s.Tap).
                Select(s => new TaplistItem
            {
                tapNumber = s.Tap,
                beerName = s.Beer.Name,
                breweryName = s.Beer.Brewery.Name,
                abv = s.Beer.Alcohol,
                bitterness = s.Beer.Ibu,
                gravity = s.Beer.Gravity,
                beerColor = getBeerColor(s.Beer),
                prices = new Dictionary<string, decimal> { {s.Volume.ToString("0.##"), s.Price} }
            });
        }

        private const string LagerBeerStyles = "01A,01B,02A,02B,02C,03A,03B,03C,03D,04A,04B,04C,06A,06B,06C,07A,08A,08B";
        private const string PorterBeerStyles = "09A,09B,09C,13C,15B,15C,16A,16B,16C,16D,20A,20B,20C";
        private const string StrongBeerStyles = "09A,09B,17A,17B,17C,17D,22B,22C,22D";
        private const string BelgianBeerStyles = "23B,23C,23D,23E,23F,24B,24C,25A,25B,25C,26A,26B,26C,26D";
        private const string WheatBeerStyles = "01D,10A,10B,10C,23A,24A";
        private const string IpaBeerStyles = "12C,18B,21A,21B,22A";

        private string getBeerColor(Beer beer)
        {
            if (beer.Brewery.Country.Id != "UA")
                return "imported";
            if (
                (!String.IsNullOrWhiteSpace(beer.BjcpStyle) && PorterBeerStyles.Contains(beer.BjcpStyle))
                || beer.Name.ToLowerInvariant().Contains("stout")
                || beer.Name.ToLowerInvariant().Contains("porter")
                )
                return "stout";
            if (
                (!String.IsNullOrWhiteSpace(beer.BjcpStyle) && IpaBeerStyles.Contains(beer.BjcpStyle))
                || beer.Name.ToLowerInvariant().Contains("ipa")
                || beer.Name.ToLowerInvariant().Contains("apa")
                )
                return "hoppy";
            if (
                (!String.IsNullOrWhiteSpace(beer.BjcpStyle) && LagerBeerStyles.Contains(beer.BjcpStyle))
                || beer.Name.ToLowerInvariant().Contains("lager")
            )
                return "lager";
            if (
                (!String.IsNullOrWhiteSpace(beer.BjcpStyle) && WheatBeerStyles.Contains(beer.BjcpStyle))
                || beer.Name.ToLowerInvariant().Contains("wheat")
            )
                return "wheat";
            if (
                (!String.IsNullOrWhiteSpace(beer.BjcpStyle) && BelgianBeerStyles.Contains(beer.BjcpStyle))
                || beer.Name.ToLowerInvariant().Contains("belgian")
            )
                return "belgian";
            if (
                (!String.IsNullOrWhiteSpace(beer.BjcpStyle) && StrongBeerStyles.Contains(beer.BjcpStyle))
                || beer.Alcohol > 8
            )
                return "strong";
                return "other";
        }
    }
}
