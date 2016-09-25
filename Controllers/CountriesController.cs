using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ontap.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    public class CountriesController : Controller
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<Country> Countries => _context.Countries;


        // GET: api/pubs
        [HttpGet]
        public IEnumerable<Country> Get() => Countries.ToArray();

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        public async Task<Country> Post([FromBody] Country country)
        {
            if (Countries.Any(c => c.Id == country.Id))
                throw new ArgumentException(string.Format("Country with id {id} already exists", country.Id));
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country;
        }

        // PUT api/cities/Kharkiv
        [HttpPut("{id}")]
        public async Task<Country> Put(string id, [FromBody]Country country)
        {
            if (Countries.All(c => c.Id != id))
                throw new KeyNotFoundException(string.Format("No country with id {id}", id));
            var currentCity = Countries.First(c => c.Id == id);
            currentCity.Name = country.Name;
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
