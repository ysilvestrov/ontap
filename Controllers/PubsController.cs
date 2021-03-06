﻿using System;
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

        #region Helpers
      
        private Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.Include(u=>u.PubAdmins).FirstAsync(u => u.Id == userId);
        }

        private async Task CheckUserRights(BeerKeg keg)
        {
            var user = await GetUser();
            if (
                //if any buyer exists, it should be pub user have access to
                keg?.Buyer == null || !user.HasRights(keg.Buyer)
            )
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
        }

        private async Task CheckUserRights(Pub pub)
        {
            if (!(await GetUser()).HasRights(pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
        }
        #endregion

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


        #region CRUD
       
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
            await CheckUserRights(pub);
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
                foreach (var keg in discontinuedBeerKegs)
                {
                    _context.BeerKegsOnTap.First(bp => bp.Id == keg.Id).DeinstallTime = DateTime.UtcNow;
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
        #endregion

        #region prices
        // GET: api/pubs/{yourpub}/prices
        [HttpGet("{id}/prices")]
        [Authorize(Policy = "PubAdminUser")]
        public IEnumerable<BeerPrice> GetPrices(string id)
        {
            var prices = _context.BeerPrices
                .Include(p => p.Beer)
                .Include(p => p.Pub)
                .Where(p => p.Pub.Id == id)
                .Where(p => (p.ValidFrom == null || p.ValidFrom < DateTime.UtcNow) && (p.ValidTo == null || p.ValidTo > DateTime.UtcNow))
                .Select(p => new BeerPrice
                {
                    Beer = new Beer {Id = p.Beer.Id},
                    Price = p.Price,
                    Volume = p.Volume
                });

            return prices.ToArray();
        }
        
        // PUT: api/pubs/{yourpub}/prices/{beer}
        [HttpPut("{id}/prices/{beerid}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerPrice> SetPrice(string id, string beerid, [FromBody] BeerPrice price)
        {
            var pub = Pubs.First(c => c.Id == id);
            await CheckUserRights(pub);

            if (price?.Beer?.Id == null)
            {
                throw new ArgumentException("Unknown beer in price", nameof(price));
            }

            var ePrice = await _context.BeerPrices
                .Include(p => p.Beer)
                .Include(p => p.Pub)
                .Where(p => (p.ValidFrom == null || p.ValidFrom < DateTime.UtcNow) && (p.ValidTo == null || p.ValidTo > DateTime.UtcNow))
                .Where(p => p.Beer.Id == price.Beer.Id && p.Pub.Id == id && p.Volume == price.Volume)
                .OrderByDescending(p => p.ValidFrom)
                .FirstOrDefaultAsync() ?? new BeerPrice();

            ePrice.Updated = DateTime.UtcNow;
            ePrice.Pub = ePrice.Pub ?? pub;
            ePrice.Beer = ePrice.Beer ?? await _context.Beers.FindAsync(beerid);
            ePrice.Price = price.Price;
            ePrice.Volume = price.Volume;
            ePrice.ValidFrom = ePrice.ValidFrom > DateTime.MinValue ? ePrice.ValidFrom : DateTime.UtcNow;
            ePrice.ValidTo = null;

            if (ePrice.Id < 1)
            {
                await _context.BeerPrices.AddAsync(ePrice);
            }

            await _context.SaveChangesAsync();

            return ePrice;
        }
        #endregion

        #region taps        
        // GET: api/pubs/{yourpub}/taps
        [HttpGet("{id}/taps")]
        [Authorize(Policy = "PubAdminUser")]
        public IEnumerable<Tap> GetTaps(string id, [FromQuery]bool pure = true)
        {
            var taps = _context.Taps
                .Include(t => t.Pub)
                .Include(t => t.BeerKegsOnTap)
                    .ThenInclude(bk => bk.Keg)
                        .ThenInclude(k => k.Beer)
                            .ThenInclude(b => b.Brewery)
                                .ThenInclude(b => b.Country)
                .Include(t => t.BeerKegsOnTap)
                    .ThenInclude(bk => bk.Keg)
                        .ThenInclude(k => k.Weights)
                .Include(t => t.BeerKegsOnTap)
                    .ThenInclude(bk => bk.Keg)
                    .ThenInclude(k => k.Keg)
                .Where(tap => tap.Pub.Id == id)
                .ToArray()
                .Select(t => new Tap
                {
                    Id = t.Id,
                    Fitting = t.Fitting,
                    HasHopinator = t.HasHopinator,
                    Number = t.Number,
                    NitrogenPercentage = t.NitrogenPercentage,
                    Status = t.Status,
                    BeerKegsOnTap =
                        t.BeerKegsOnTap
                        .Where(bk => bk.DeinstallTime == null || bk.DeinstallTime < DateTime.UtcNow)
                        .Select(bk => new BeerKegOnTap
                            {
                                Id = bk.Id,
                                DeinstallTime = bk.DeinstallTime,
                                InstallTime = bk.InstallTime,
                                Priority = bk.Priority,
                                Keg = new BeerKeg
                                {
                                    Id = bk.Keg.Id,
                                    ArrivalDate = bk.Keg.ArrivalDate,
                                    Beer = bk.Keg.Beer,
                                    BestBeforeDate = bk.Keg.BestBeforeDate,
                                    BrewingDate = bk.Keg.BrewingDate,
                                    DeinstallationDate = bk.Keg.DeinstallationDate,
                                    InstallationDate = bk.Keg.InstallationDate,
                                    PackageDate = bk.Keg.PackageDate,
                                    Keg = new Keg {
                                        Id = bk.Keg.Keg.Id,                                        
                                        EmptyWeight = bk.Keg.Keg.EmptyWeight,                                        
                                        ExternalId = bk.Keg.Keg.ExternalId,                                        
                                        Fitting = bk.Keg.Keg.Fitting,                                        
                                        IsReturnable = bk.Keg.Keg.IsReturnable,                                        
                                        Material = bk.Keg.Keg.Material,                                        
                                        Volume = bk.Keg.Keg.Volume,                                        
                                    },
                                    Status = bk.Keg.Status,
                                    Weights = bk.Keg.Weights.Select(w => new BeerKegWeight
                                    {
                                      Id  = w.Id,
                                      Date = w.Date,
                                      Weight = w.Weight
                                    }).ToArray()
                                }
                            })
                            .ToArray()
                });
            return taps;
        }
        #endregion

        #region queue
        // GET: api/pubs/{yourpub}/queue
        [HttpGet("{id}/queue")]
        [Authorize(Policy = "PubAdminUser")]
        public IEnumerable<BeerKegOnTap> GetQueue(string id, [FromQuery]bool pure = true)
        {
            var kegsOnTapInQueue = GetKegsOnTapInQueue(id);

            var kegs = ClearBeerKegOnTaps(kegsOnTapInQueue);    
            return kegs;
        }

        private static BeerKegOnTap[] ClearBeerKegOnTaps(BeerKegOnTap[] kegsOnTapInQueue)
        {
            var kegs = kegsOnTapInQueue
                .Select(bk => new BeerKegOnTap
                {
                    Id = bk.Id,
                    DeinstallTime = bk.DeinstallTime,
                    InstallTime = bk.InstallTime,
                    Priority = bk.Priority,
                    Keg = new BeerKeg
                    {
                        Id = bk.Keg.Id,
                        ArrivalDate = bk.Keg.ArrivalDate,
                        Beer = bk.Keg.Beer,
                        BestBeforeDate = bk.Keg.BestBeforeDate,
                        BrewingDate = bk.Keg.BrewingDate,
                        DeinstallationDate = bk.Keg.DeinstallationDate,
                        InstallationDate = bk.Keg.InstallationDate,
                        PackageDate = bk.Keg.PackageDate,
                        Keg = new Keg
                        {
                            Id = bk.Keg.Keg.Id,
                            EmptyWeight = bk.Keg.Keg.EmptyWeight,
                            ExternalId = bk.Keg.Keg.ExternalId,
                            Fitting = bk.Keg.Keg.Fitting,
                            IsReturnable = bk.Keg.Keg.IsReturnable,
                            Material = bk.Keg.Keg.Material,
                            Volume = bk.Keg.Keg.Volume,
                        },
                        Status = bk.Keg.Status,
                        Weights = bk.Keg.Weights
                    }
                })
                .ToArray();
            return kegs;
        }

        private BeerKegOnTap[] GetKegsOnTapInQueue(string id)
        {
            var kegsBoughtAndNotInstalled = _context.Pubs
                .Include(p => p.BeerKegsBought)
                .ThenInclude(k => k.Beer)
                .ThenInclude(b => b.Brewery)
                .ThenInclude(b => b.Country)
                .Include(p => p.BeerKegsBought)
                .ThenInclude(k => k.Weights)
                .First(p => p.Id == id)
                .BeerKegsBought
                .Where(bk => bk.InstallationDate == null || bk.InstallationDate > DateTime.UtcNow)
                .ToArray();

            var kegsOnTapInQueue = _context.BeerKegsOnTap
                //.Include(bk => bk.Tap)
                .Include(bk => bk.Keg)
                .ThenInclude(bk => bk.Keg)
                .Where(bk => bk.Tap == null).ToArray();

            kegsOnTapInQueue = kegsOnTapInQueue.Where(kot => kegsBoughtAndNotInstalled.Any(k => kot.Keg.Id == k.Id)).ToArray();
            return kegsOnTapInQueue;
        }

        // POST: api/pubs/{yourpub}/queue
        [HttpPost("{id}/queue")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKeg> AddToQueue(string id, [FromBody] BeerKeg keg)
        {
            var kegId = keg.Id;
            keg = await _context.BeerKegs.Include(k=>k.Buyer).FirstAsync(k => k.Id == kegId);

            await CheckUserRights(keg);

            var beerKegOnTap = new BeerKegOnTap
            {
                Priority = 1,
                Keg = _context.BeerKegs.FirstOrDefault(k => k.Id == kegId)
            };
            _context.BeerKegsOnTap.Add(beerKegOnTap);

            await _context.SaveChangesAsync();

            Task<BeerKeg> result =
                 _context.BeerKegs.Include(k => k.BeerKegsOnTap).FirstAsync(k => k.Id == kegId);

            await result.ContinueWith(t =>
            {
                var bk = t.Result;
                bk.BeerKegsOnTap = bk.BeerKegsOnTap
                    .Where(bko => bko.InstallTime == null || bko.InstallTime > DateTime.UtcNow)
                    .Select(bko => new BeerKegOnTap
                    {
                        Id = bko.Id,
                        InstallTime = bko.InstallTime,
                        DeinstallTime = bko.DeinstallTime,
                        Priority = bko.Priority
                    }).ToArray();
                return bk;
            });

            return await result;
        }

        // POST: api/pubs/{yourpub}/reorder-queue
        [HttpPost("{id}/reorder-queue/{priority}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<IEnumerable<BeerKegOnTap>> ReorderQueue(string id, int priority, [FromBody] BeerKegOnTap keg)
        {
            var beerKeg = await _context.BeerKegs.Include(k=>k.Buyer).FirstAsync(k => k.Id == keg.Keg.Id);

            await CheckUserRights(beerKeg);

            var kegsOnTapInQueue = GetKegsOnTapInQueue(id).OrderBy(k => k.Priority).ToArray();
            var temp = new List<BeerKegOnTap>(kegsOnTapInQueue.Length);
            if (keg.Priority < priority)
            {
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority < keg.Priority));
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority > keg.Priority && k.Priority < priority));
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority == keg.Priority));
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority >= priority));
            }
            else
            {
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority < priority));
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority == keg.Priority));
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority >= priority && k.Priority < keg.Priority));
                temp.AddRange(kegsOnTapInQueue.Where(k => k.Priority > keg.Priority));
            }

            for (int i = 0; i < temp.Count; i++)
            {
                var kegOnTap = temp[i];
                if (kegOnTap.Priority != i)
                {
                    var tkeg = await _context.BeerKegsOnTap.FirstOrDefaultAsync(k => k.Id == kegOnTap.Id);
                    if (tkeg != null)
                    {
                        tkeg.Priority = i;
                    }
                }
                kegOnTap.Priority = i;
            }

            await _context.SaveChangesAsync();

            var kegs = ClearBeerKegOnTaps(temp.ToArray());
            return kegs;
        }
        #endregion

        #region storage
        // GET: api/pubs/{yourpub}/storage
        [HttpGet("{id}/storage")]
        [Authorize(Policy = "PubAdminUser")]
        public IEnumerable<BeerKeg> GetStorage(string id, [FromQuery]bool pure = true)
        {
            var kegsBoughtAndNotInstalled = _context.Pubs
                .Include(p => p.BeerKegsBought)
                    .ThenInclude(k => k.Beer)
                        .ThenInclude(b => b.Brewery)
                            .ThenInclude(b => b.Country)
                .Include(p => p.BeerKegsBought)
                    .ThenInclude(k => k.Weights)
                .Include(p => p.BeerKegsBought)
                    .ThenInclude(k => k.Keg)
                .Include(p => p.BeerKegsBought)
                    .ThenInclude(k => k.BeerKegsOnTap)
                    .ThenInclude(bko => bko.Tap)
                .First(p => p.Id == id)
                .BeerKegsBought
                .Where(bk => bk.InstallationDate == null || bk.InstallationDate > DateTime.UtcNow)
                .ToArray();

            var kegs = kegsBoughtAndNotInstalled
                .Select(bk => new BeerKeg
                    {
                        Id = bk.Id,
                        ArrivalDate = bk.ArrivalDate,
                        Beer = bk.Beer,
                        BestBeforeDate = bk.BestBeforeDate,
                        BrewingDate = bk.BrewingDate,
                        DeinstallationDate = bk.DeinstallationDate,
                        InstallationDate = bk.InstallationDate,
                        PackageDate = bk.PackageDate,
                        Keg = new Keg
                        {
                            Id = bk.Keg.Id,
                            EmptyWeight = bk.Keg.EmptyWeight,
                            ExternalId = bk.Keg.ExternalId,
                            Fitting = bk.Keg.Fitting,
                            IsReturnable = bk.Keg.IsReturnable,
                            Material = bk.Keg.Material,
                            Volume = bk.Keg.Volume,
                        },
                        Status = bk.Status,
                        Weights = bk.Weights,
                        BeerKegsOnTap = bk.BeerKegsOnTap
                            .Where(bko => bko.InstallTime == null || bko.InstallTime > DateTime.UtcNow )
                            .Select(bko => new BeerKegOnTap
                                {
                                    Id = bko.Id,
                                    InstallTime = bko.InstallTime,
                                    DeinstallTime = bko.DeinstallTime,
                                    Priority = bko.Priority,
                                    Tap = bko.Tap == null ? null : new Tap()
                                }).ToArray()

                    })
                .ToArray();
            return kegs;
        }

        // DELETE: api/pubs/{yourpub}/storage
        [HttpDelete("{id}/storage/{kegId}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKeg> RemoveFromStorage(string id, int kegId)
        {
            var pub = _context.Pubs
                .Include(p => p.BeerKegsBought)
                .ThenInclude(k => k.Beer)
                .ThenInclude(b => b.Brewery)
                .Include(p => p.BeerKegsBought)
                .ThenInclude(k => k.Weights)
                .Include(p => p.BeerKegsBought)
                .ThenInclude(k => k.Keg)
                .Include(p => p.BeerKegsBought)
                .ThenInclude(k => k.BeerKegsOnTap)
                .First(p => p.Id == id);

            var kegsBoughtAndNotInstalled = pub
                .BeerKegsBought
                .Where(bk => bk.InstallationDate == null || bk.InstallationDate > DateTime.UtcNow);

            var keg = kegsBoughtAndNotInstalled.FirstOrDefault(k => k.Id == kegId);

            if (keg == null)
                throw new KeyNotFoundException($"No keg with id {kegId} at pub {id} storage");

            await CheckUserRights(pub);

            keg.InstallationDate = DateTime.UtcNow.AddDays(-1);
            if (keg.DeinstallationDate == null)
            {
                keg.DeinstallationDate = DateTime.UtcNow;
            }
            foreach (var bko in keg.BeerKegsOnTap)
            {
                if (bko.InstallTime == null)
                    bko.InstallTime = DateTime.UtcNow.AddDays(-1);
                if (bko.DeinstallTime == null)
                    bko.DeinstallTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return keg;
        }

        public struct NBeerKeg
        {
            public BeerKeg keg;
            public int count;
        }

        // POST: api/pubs/{yourpub}/storage
        [HttpPost("{id}/storage")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKeg[]> AddToStorage(string id, [FromBody] NBeerKeg nBeerKeg)
        {
            if (Pubs.All(c => c.Id != id))
                throw new KeyNotFoundException($"No pub with id {id}");
            var pub = Pubs.First(c => c.Id == id);
            if (!(await GetUser()).HasRights(pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            var keg = nBeerKeg.keg;
            var count = nBeerKeg.count;

            keg.Buyer = pub;
            if (!String.IsNullOrWhiteSpace(keg.Beer?.Id))
            {
                keg.Beer = await _context.Beers.Include(b => b.Brewery).FirstOrDefaultAsync(b => b.Id == keg.Beer.Id);
            }
            else if (!String.IsNullOrWhiteSpace(keg.Beer?.Brewery?.Id))
            {
                keg.Beer.Brewery = await _context.Breweries
                    .FirstOrDefaultAsync(b => b.Id == keg.Beer.Brewery.Id);
            }
            else if (keg.Beer == null)
            {
                //beer is unknown
                keg.Beer = await _context.Beers.Include(b => b.Brewery).FirstOrDefaultAsync(b => b.Id == DataContext.UnknownBeerId);
            }
            
            keg.InstallationDate = null;
            keg.DeinstallationDate = null;
            keg.ArrivalDate = keg.ArrivalDate ?? DateTime.UtcNow;
            keg.Status = KegStatus.Waiting;
            foreach (var beerKegWeight in keg.Weights)
            {
                if (beerKegWeight.Id < 1)
                {
                    beerKegWeight.Date = DateTime.UtcNow;
                    beerKegWeight.Keg = keg;
                }
            }
            var kegs = new BeerKeg[count];
            for (var i = 0; i < count; i++)
            {
                kegs[i] = new BeerKeg
                {
                    ArrivalDate = keg.ArrivalDate,
                    Beer = keg.Beer,
                    BestBeforeDate = keg.BestBeforeDate,
                    BrewingDate = keg.BrewingDate,
                    Buyer = keg.Buyer,
                    DeinstallationDate = keg.DeinstallationDate,
                    InstallationDate = keg.InstallationDate,
                    PackageDate = keg.PackageDate,
                    Status = keg.Status,
                    Keg = new Keg
                    {
                        EmptyWeight = keg.Keg.EmptyWeight,
                        ExternalId = keg.Keg.ExternalId,
                        Fitting = keg.Keg.Fitting,
                        IsReturnable = keg.Keg.IsReturnable,
                        Material = keg.Keg.Material,
                        Volume = keg.Keg.Volume,
                    }
                };
                kegs[i].Weights = keg.Weights.Select(w =>
                    new BeerKegWeight {Keg = kegs[i], Date = w.Date, Weight = w.Weight}).ToList();

                await _context.BeerKegs.AddAsync(kegs[i]);
            }
            await _context.SaveChangesAsync();
            return kegs;
        }

        // PUT: api/pubs/{yourpub}/storage/{kegid}
        [HttpPut("{id}/storage/{kegid}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<BeerKeg> AddToStorage(string id, int kegid, [FromBody] BeerKeg beerKeg)
        {
            if (Pubs.All(c => c.Id != id))
                throw new KeyNotFoundException($"No pub with id {id}");
            var pub = Pubs.First(c => c.Id == id);
            if (!(await GetUser()).HasRights(pub))
            {
                throw new InvalidCredentialException("Current user has no right to change this record");
            }
            if (beerKeg.Id != kegid || pub.BeerKegsBought.All(k => k.Id != kegid))
                throw new InvalidCredentialException("Current user has no right to change this record");

            var keg = pub.BeerKegsBought.First(k => k.Id == kegid);

            keg.Beer = beerKeg.Beer;

            if (!String.IsNullOrWhiteSpace(keg.Beer?.Id))
            {
                keg.Beer = await _context.Beers.Include(b => b.Brewery).FirstOrDefaultAsync(b => b.Id == keg.Beer.Id);
            }
            else if (!String.IsNullOrWhiteSpace(keg.Beer?.Brewery?.Id))
            {
                keg.Beer.Brewery = await _context.Breweries
                    .FirstOrDefaultAsync(b => b.Id == keg.Beer.Brewery.Id);
            }
            
            keg.InstallationDate = beerKeg.InstallationDate;
            keg.DeinstallationDate = beerKeg.DeinstallationDate;
            keg.ArrivalDate = beerKeg.ArrivalDate;
            keg.Status = beerKeg.Status;
            keg.PackageDate = beerKeg.PackageDate;
            keg.BrewingDate = beerKeg.BrewingDate;

            foreach (var beerKegWeight in beerKeg.Weights)
            {
                if (beerKegWeight.Id >= 1) continue;
                beerKegWeight.Date = DateTime.UtcNow;
                beerKegWeight.Keg = keg;
                keg.Weights.Add(beerKegWeight);
            }

            await _context.SaveChangesAsync();
            return keg;
        }
        #endregion
    }
}