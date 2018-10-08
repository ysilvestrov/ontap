using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Ontap.Auth;
using Ontap.Models;

namespace Ontap.Controllers
{
    [Route("api/[controller]")]
    public class JwtController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly DataContext _context;

        public JwtController(IOptions<JwtIssuerOptions> jwtOptions, ILoggerFactory loggerFactory, DataContext context)
        {
            _context = context;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<JwtController>();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] UserBase applicationUser)
        {
            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                _logger.LogInformation(
                    $"Invalid username ({applicationUser.Name}) or password ({applicationUser.Password})");
                return BadRequest("Invalid credentials");
            }

            var claims = new List<Claim>();
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Name),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    ToUnixEpochDate(DateTime.Now).ToString(),
                    ClaimValueTypes.Integer64)
            });
            claims.AddRange(identity.FindAll("UserType"));
            claims.AddRange(identity.FindAll("PubAdmin"));
            claims.AddRange(identity.FindAll("BreweryAdmin"));
            

            // Create the JWT security token and encode it.
            var validForTotalSeconds = (int)_jwtOptions.ValidFor.TotalSeconds;
            var expiresAt = DateTime.UtcNow.AddSeconds(validForTotalSeconds);

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: expiresAt,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var accessToken = new AccessToken
            {
                accessToken = encodedJwt,
                expiresIn = validForTotalSeconds,
                expiresAt = expiresAt
            };

            //var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return Ok(accessToken);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private Task<ClaimsIdentity> GetClaimsIdentity(UserBase user)
        {
            var hash = UserBase.GetHash(user.Password);
            var found =
                _context.Users.FirstOrDefault(u => (u.Id == user.Name || u.Email == user.Name) && u.Password == hash);

            if (found == null)
                return Task.FromResult<ClaimsIdentity>(null);

            var claims = new List<Claim>();

            if (found.CanAdminPub)
            {
                claims.Add(new Claim("UserType", "PubAdmin"));
                claims.AddRange(
                    _context.PubAdmins.Where(pu => pu.User.Id == found.Id)
                        .Select(pu => new Claim("PubAdmin", pu.Pub.Id)));
            }
            if (found.CanAdminBrewery)
            {
                claims.Add(new Claim("UserType", "BreweryAdmin"));
            }
            if (found.IsAdmin)
            {
                claims.Add(new Claim("UserType", "Admin"));
                if (!found.CanAdminPub)
                {
                    claims.AddRange(_context.Pubs.Select(p => new Claim("PubAdmin", p.Id)));
                }
            }
            return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.Name, "Token"), claims));
        }

        public class AccessToken
        {
            // ReSharper disable InconsistentNaming
            public string accessToken { get; set; }
            public DateTime expiresAt { get; set; }
            public int expiresIn { get; set; }
            // ReSharper restore InconsistentNaming
        }
    }
}
