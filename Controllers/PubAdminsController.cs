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
    [Authorize(Policy = "AdminUser")]
    public class PubAdminsController : Controller
    {
        private readonly DataContext _context;

        public PubAdminsController(DataContext context)
        {
            _context = context;
  
        }

        public IEnumerable<PubAdmin> Admins => _context.PubAdmins
            .Include(pa => pa.User)
            .Include(pa => pa.Pub)
            .ToArray();


        // GET: api/serves
        [HttpGet]
        public IEnumerable<PubAdmin> Get() => Admins as PubAdmin[] ?? Admins.ToArray();

        // POST api/serves
        /// <exception cref="ArgumentException">Already existing record.</exception>
        [HttpPost]
        public async Task<PubAdmin> Post([FromBody] PubAdmin admin)
        {
            if (Admins.Any(s => s.User.Id == admin.User.Id && s.Pub.Id == admin.Pub.Id))
                throw new AlreadyExistsException("Record for the same user and pub already exists");
            var current = new PubAdmin
            {
                User = _context.Users.First(user => user.Id == admin.User.Id),
                Pub = _context.Pubs.First(pub => pub.Id == admin.Pub.Id)
            };
            _context.PubAdmins.Add(current);
            await _context.SaveChangesAsync();
            return Admins.First(s => s.User.Id == admin.User.Id && s.Pub.Id == admin.Pub.Id);
        }

        // PUT api/serves/123
        /// <exception cref="KeyNotFoundException">Record not found.</exception>
        [HttpPut("{id}")]
        public async Task<PubAdmin> Put(int id, [FromBody]PubAdmin admin)
        { 
            if (Admins.All(c => c.Id != id))
                throw new KeyNotFoundException($"No record with id {id}");
            var current = Admins.First(c => c.Id == id);
            current.User = _context.Users.First(user => user.Id == admin.User.Id);
            current.Pub = _context.Pubs.First(pub => pub.Id == admin.Pub.Id);
            await _context.SaveChangesAsync();
            return current;
        }

        // Delete api/serves/123
        /// <exception cref="KeyNotFoundException">Recod not found.</exception>
        [HttpDelete("{id}")]
        public async Task<PubAdmin> Delete(int id)
        { 
            if (Admins.All(c => c.Id != id))
                throw new KeyNotFoundException($"No record with id {id}");
            var current = Admins.First(c => c.Id == id);
            _context.PubAdmins.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
