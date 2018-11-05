using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ontap.Models;

namespace Ontap.Controllers
{
    public class VolumePrice
    {
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }
    public class Serve
    {
        public string Tap { get; set; }
        public Beer Beer { get; set; }
        public IEnumerable<VolumePrice> Prices { get; set; }
    }

    public class PubServe : Pub
    {
        public PubServe(Pub pub)
        {
            Id = pub.Id;
            Name = pub.Name;
            Address = pub.Address;
            City = pub.City;
            Admins = pub.Admins;
            Image = pub.Image;
            TaplistHeaderImage = pub.TaplistHeaderImage;
            TaplistFooterImage = pub.TaplistFooterImage;
            FacebookUrl = pub.FacebookUrl;
            VkontakteUrl = pub.VkontakteUrl;
            WebsiteUrl = pub.WebsiteUrl;
            BookingUrl = pub.BookingUrl;
            ParserOptions = pub.ParserOptions;
            TapNumber = pub.TapNumber;
            Taps = pub.Taps;
            BeerPrices = pub.BeerPrices;
            BeerKegsBought = pub.BeerKegsBought;
        }

        [JsonProperty("serves")]
        public IEnumerable<Serve> BeersServedInPub
        {
            get
            {
                IEnumerable<Serve> ret = new Serve[0];
                if (Taps != null && Taps.Count > 0)
                    ret = Taps.Select(t => new
                        {
                            tap = t.Number,
                            bk = t.BeerKegsOnTap
                                .Where(bk => bk.DeinstallTime == null || bk.DeinstallTime > DateTime.UtcNow)
                                .OrderBy(bk => bk.InstallTime).LastOrDefault()
                        })
                        .Where(li => li.bk != null)
                        .Select(li => new Serve
                        {
                            Tap = li.tap,
                            Beer = li.bk.Keg.Beer,
                            Prices = BeerPrices.Where(bp => bp.Beer.Id == li.bk.Keg.Beer.Id)
                                .Select(bp => new VolumePrice {Price = bp.Price, Volume = bp.Volume})
                        });
                return ret;
            }
        }

        public DateTime LastUpdated
        {
            get
            {
                return (BeerPrices == null || BeerPrices.Count <= 0 || !BeerPrices.Any(bp =>
                            bp.Pub.Id == Id && (bp.ValidTo == null || bp.ValidTo > DateTime.UtcNow)))
                    ? DateTime.MinValue
                    : BeerPrices.Where(bp => bp.Pub.Id == Id && (bp.ValidTo == null || bp.ValidTo > DateTime.UtcNow))
                        .Max(bp => bp.Updated);
            }
        }
    }

    [Route("api/pubserves")]
    public class PubServesController
    {
        private DataContext _context;

        public PubServesController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
        }

        private IEnumerable<PubServe> Pubs => _context.Pubs
            .Include(p => p.City)
            .Include(p => p.Taps)
            .ThenInclude(t => t.BeerKegsOnTap)
            .ThenInclude(bkt => bkt.Keg)
            .ThenInclude(k => k.Beer)
            .ThenInclude(b => b.Brewery)
            .ThenInclude(b => b.Country)
            .Include(p => p.BeerPrices)
            .ThenInclude(s => s.Beer)
            .ThenInclude(b => b.Brewery)
            .ThenInclude(b => b.Country)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .Select(p => new PubServe(p))
            .ToArray();

        // GET: api/pubs
        [HttpGet]
        public IEnumerable<Pub> Get()
        {
            var pubs = Pubs;
            var enumerable = pubs as PubServe[] ?? pubs.ToArray();
            foreach (var pub in enumerable)
            {
                pub.BeersServedInPub.ToArray();
                //pub.BeerPrices = null;
                //pub.Admins = null;
                //pub.BeerKegsBought = null;
            }
            return enumerable;
        }

        // GET: api/pubs/id
        [HttpGet("{id}")]
        public Pub Get(string id)
        {
            var pub = Pubs.First(p => p.Id == id);
            pub.BeersServedInPub.ToArray();
            return pub;
        }
    }
}
