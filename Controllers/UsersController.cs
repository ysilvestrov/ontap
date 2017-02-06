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
    public class UsersController : Controller
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
  
        }

        private IEnumerable<User> Users => _context.Users
            .Include(u => u.PubAdmins)
            .ThenInclude(pa => pa.Pub)
            .Include(u => u.BreweryAdmins)
            .ThenInclude(ba => ba.Brewery)
            .ToArray().Select(u =>
            {
                u.Password = "";
                return u;
            });


        // GET: api/pubs
        [HttpGet]
        [Authorize(Policy = "AdminUser")]
        public IEnumerable<User> Get()
        {
            return Users;
        }

        // GET: api/pubs
        [HttpGet("{id}")]
        [Authorize]
        public User Get(string id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
        }

        // POST api/values
        /// <exception cref="ArgumentException">If user with the same Id already exists.</exception>
        [HttpPost]
        [Authorize(Policy = "AdminUser")]
        public async Task<User> Post([FromBody] User user)
        {
            if (_context.Users.Any(c => c.Id == user.Id))
                throw new AlreadyExistsException($"User with id {user.Id} already exists");
            user.Password = UserBase.GetHash(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // PUT api/cities/Kharkiv
        /// <exception cref="KeyNotFoundException">User with given Id does not exists.</exception>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminUser")]
        public async Task<User> Put(string id, [FromBody]User user)
        { 
            if (_context.Users.All(c => c.Id != id))
                throw new KeyNotFoundException($"No user with id {id}");
            var current = _context.Users.First(c => c.Id == id);
            current.Name = user.Name;
            current.Password = string.IsNullOrWhiteSpace(user.Password)
                ? current.Password
                : UserBase.GetHash(user.Password);
            current.IsAdmin = user.IsAdmin;
            current.CanAdminPub = user.CanAdminPub;
            current.CanAdminBrewery = user.CanAdminBrewery;          
            await _context.SaveChangesAsync();
            return current; 
        }
    }
}