using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Datatypes;
using NickBuhro.Translit;
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

        public PubsController(DataContext context)
        {
            _context = context;
  
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
            var pid = pub.Name.MakeId();
            var id = pid;

            for (var i = 0; Pubs.Any(c => c.Id == id); id = $"{pid}_{i}", i++);

            pub.Id = id;
            _context.Pubs.Add(pub);
            await _context.SaveChangesAsync();
            return await _context.Pubs.FirstAsync(p => p.Id == id);
        }

        // PUT api/cities/Kharkiv
        [HttpPut("{id}")]
        [Authorize(Policy = "PubAdminUser")]
        public async Task<Pub> Put(string id, [FromBody]Pub pub)
        { 
            if (Pubs.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No pub with id {id}", id));
            var current = Pubs.First(c => c.Id == id);
            current.Name = pub.Name;
            current.City = _context.Cities.First(p => p.Id == pub.City.Id);
            current.Address = pub.Address;
            current.Image = pub.Image;
            current.BookingUrl = pub.BookingUrl;
            current.FacebookUrl = pub.FacebookUrl;
            current.VkontakteUrl = pub.VkontakteUrl;
            current.WebsiteUrl = pub.WebsiteUrl;
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
