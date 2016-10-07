using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ontap.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BeerServedInPubs>()
                .HasOne(p => p.Served)
                .WithMany(b => b.BeerServedInPubs)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BeerServedInPubs>()
                .HasOne(p => p.ServedIn)
                .WithMany(b => b.BeerServedInPubs)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BreweryAdmin>()
                .HasOne(ba => ba.User)
                .WithMany(u => u.BreweryAdmins)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BreweryAdmin>()
                .HasOne(ba => ba.Brewery)
                .WithMany(b => b.Admins)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PubAdmin>()
                .HasOne(ba => ba.User)
                .WithMany(u => u.PubAdmins)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PubAdmin>()
                .HasOne(ba => ba.Pub)
                .WithMany(b => b.Admins)
                .OnDelete(DeleteBehavior.Cascade);

        }

        public void EnsureSeedData(IConfigurationRoot configuration)
        {
            EnsurePubData();
            EnsureUserData(configuration);
        }

        private void EnsureUserData(IConfigurationRoot configuration)
        {
            if (Users.Any()) return;

            var admin = new User
            {
                Name = "admin",
                Id = "admin",
                IsAdmin = true,
                Password = configuration.GetValue<string>("DefaultAdminPassword")
            };

            Users.Add(admin);
            SaveChanges();
        }

        private void EnsurePubData()
        {
            if (Breweries.Any()) return;
            var ukraine = new Country {Id = "UA", Name = "Ukraine"};
            var syndicateBrewery = new Brewery {Id = "Syndicate", Name = "Syndicate", Country = ukraine};
            var whiteRabbitBrewery = new Brewery {Id = "White Rabbit", Name = "White Rabbit", Country = ukraine};
            var abbeyDubbelBeer = new Beer
            {
                Name = "Rose's Name",
                Id = "Syndicate Beer Rose’s Name",
                Kind = Beer.Classification.Ale,
                Alcohol = 5.8M,
                Brewery = syndicateBrewery,
                Description =
                    "заряд янтарного милосердия от киевского-ресторана пивоварни Синдикат. Прекрасный, чуть сладкий, янтарный эль, располагающий к меланхолии переулков каменных городков старушки-Бельгии",
                Ibu = 16,
                Type = "Abbey Dubbel"
            };
            var mildAleBeer = new Beer
            {
                Name = "Mild Ale",
                Id = "Syndicate Beer Mild Ale",
                Kind = Beer.Classification.Ale,
                Alcohol = 3.5M,
                Brewery = syndicateBrewery,
                Description =
                    "лучшая характеристика этого пива - “Шахтерский эль”. Легкое, темное, чуть карамельное пиво, которым приятно ударить по усталости в конце рабочей недели и по мозолям от пробелов и тачпадов.",
                Ibu = 18,
                Type = "English Mild Ale"
            };
            var sanDiegoApaBeer = new Beer
            {
                Name = "San-Diego APA",
                Id = "White Rabbit Beer San-Diego APA",
                Kind = Beer.Classification.Ale,
                Alcohol = 5.5M,
                Gravity = 12.5M,
                Brewery = whiteRabbitBrewery,
                Description =
                    "эль названный в честь штамма дрожжей, названного в честь города в котором был доведен до совершенства такой сорт как двойной IPA. Сам SD конечно ничего общего с этими ипа не имеет, но его приятная охмеленность отлично отдает дань американским элям.",
                Ibu = 34,
                Type = "American Pale Ale"
            };

            var kharkiv = new City {Id = "Kharkiv", Name = "Харьков"};
            var kyiv = new City {Id = "Kyiv", Name = "Киев"};

            var redDoorPub = new Pub
            {
                Id = "Red Door Pub",
                Name = "Red Door Паб",
                Address = "ул. Гоголя 2а",
                City = kharkiv,
                //Serves = new List<Beer>{AbbeyDubbelBeer, MildAleBeer, SanDiegoApaBeer}
            };

            var mohnatyyHmilPub = new Pub
            {
                Id = "Mohnatyy Hmil",
                Name = "Мохнатый Хмель",
                Address = "ул. Велика Васильківська, 126",
                City = kyiv,
                //Serves = new List<Beer>{AbbeyDubbelBeer, MildAleBeer}
            };

            BeerServedInPubs.RemoveRange(
                BeerServedInPubs.Where(s =>
                        s.ServedIn.Id == mohnatyyHmilPub.Id || s.ServedIn.Id == redDoorPub.Id ||
                        s.Served.Id == abbeyDubbelBeer.Id || s.Served.Id == sanDiegoApaBeer.Id ||
                        s.Served.Id == mildAleBeer.Id
                ));
            Pubs.RemoveRange(Pubs.Where(p => p.Id == mohnatyyHmilPub.Id || p.Id == redDoorPub.Id));
            Beers.RemoveRange(
                Beers.Where(b => b.Id == abbeyDubbelBeer.Id || b.Id == sanDiegoApaBeer.Id || b.Id == mildAleBeer.Id));
            Cities.RemoveRange(Cities.Where(c => c.Id == kyiv.Id || c.Id == kharkiv.Id));
            SaveChanges();

            Beers.AddRange(abbeyDubbelBeer, sanDiegoApaBeer, mildAleBeer);
            Cities.AddRange(kyiv, kharkiv);
            SaveChanges();

            Pubs.AddRange(mohnatyyHmilPub, redDoorPub);
            SaveChanges();

            BeerServedInPubs.AddRange(
                new BeerServedInPubs {Served = abbeyDubbelBeer, ServedIn = mohnatyyHmilPub, Price = 40},
                new BeerServedInPubs {Served = mildAleBeer, ServedIn = mohnatyyHmilPub, Price = 40},
                new BeerServedInPubs {Served = abbeyDubbelBeer, ServedIn = redDoorPub, Price = 50},
                new BeerServedInPubs {Served = mildAleBeer, ServedIn = redDoorPub, Price = 45},
                new BeerServedInPubs {Served = sanDiegoApaBeer, ServedIn = redDoorPub, Price = 50}
            );

            SaveChanges();
        }

        public DbSet<Pub> Pubs { get; set; }
        public DbSet<Beer> Beers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<BeerServedInPubs> BeerServedInPubs { get; set; }
        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PubAdmin> PubAdmins { get; set; }
        public DbSet<BreweryAdmin> BreweryAdmins { get; set; }
    }
}