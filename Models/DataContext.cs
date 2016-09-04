using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
        }

        public void EnsureSeedData()
        {
            if (Pubs.Any()) return;
            var abbeyDubbelBeer = new Beer
            {
                Name = "Rose's Name",
                Id = "Syndicate Beer Rose’s Name",
                Kind = Beer.Classification.Ale,
                Alcohol = 5.8M,
                Brewery = "Syndicate",
                Description = "заряд янтарного милосердия от киевского-ресторана пивоварни Синдикат. Прекрасный, чуть сладкий, янтарный эль, располагающий к меланхолии переулков каменных городков старушки-Бельгии",
                Ibu = 16,
                Type = "Abbey Dubbel"
            };
            var mildAleBeer = new Beer
            {
                Name = "Mild Ale",
                Id = "Syndicate Beer Mild Ale",
                Kind = Beer.Classification.Ale,
                Alcohol = 3.5M,
                Brewery = "Syndicate",
                Description = "лучшая характеристика этого пива - “Шахтерский эль”. Легкое, темное, чуть карамельное пиво, которым приятно ударить по усталости в конце рабочей недели и по мозолям от пробелов и тачпадов.",
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
                Brewery = "White Rabbit",
                Description = "эль названный в честь штамма дрожжей, названного в честь города в котором был доведен до совершенства такой сорт как двойной IPA. Сам SD конечно ничего общего с этими ипа не имеет, но его приятная охмеленность отлично отдает дань американским элям.",
                Ibu = 34,
                Type = "American Pale Ale"
            };

            var kharkiv = new City { Id = "Kharkiv", Name = "Харьков" };
            var kyiv = new City { Id = "Kyiv", Name = "Киев" };

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


            Beers.AddRange(abbeyDubbelBeer, sanDiegoApaBeer, mildAleBeer);
            Cities.AddRange(kyiv, kharkiv);
            SaveChanges();

            Pubs.AddRange(mohnatyyHmilPub, redDoorPub);
            SaveChanges();

            BeerServedInPubs.AddRange(
                new BeerServedInPubs {Served = abbeyDubbelBeer, ServedIn = mohnatyyHmilPub},
                new BeerServedInPubs {Served = mildAleBeer, ServedIn = mohnatyyHmilPub},
                new BeerServedInPubs {Served = abbeyDubbelBeer, ServedIn = redDoorPub},
                new BeerServedInPubs {Served = mildAleBeer, ServedIn = redDoorPub},
                new BeerServedInPubs {Served = sanDiegoApaBeer, ServedIn = redDoorPub}
                );

            SaveChanges();

        }

        public DbSet<Pub> Pubs { get; set; }
        public DbSet<Beer> Beers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<BeerServedInPubs> BeerServedInPubs { get; set; }
    }
}