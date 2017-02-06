using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ontap.Auth;
using Ontap.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    public class CitiesController : Controller
    {
        private readonly DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<City> Cities => _context.Cities;


        // GET: api/pubs
        [HttpGet]
        public IEnumerable<City> Get() => Cities.ToArray();

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        [Authorize(Policy = "AdminUser")]
        public async Task<City> Post([FromBody] City city)
        {
            if (Cities.Any(c => c.Id == city.Id))
                throw new AlreadyExistsException($"City with id {city.Id} already exists");
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return city;
        }

        // PUT api/cities/Kharkiv
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<City> Put(string id, [FromBody]City city)
        {
            if (Cities.All(c => c.Id != id))
                throw new KeyNotFoundException($"No city with id {id}");
            var currentCity = Cities.First(c => c.Id == id);
            currentCity.Name = city.Name;
            await _context.SaveChangesAsync();
            return currentCity;
        }

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
