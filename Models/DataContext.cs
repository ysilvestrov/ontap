using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Ontap.Models
{
    public interface IDataContext
    {
        void EnsureSeedData(IConfigurationRoot configuration);
        DbSet<Pub> Pubs { get; set; }
        DbSet<Beer> Beers { get; set; }
        DbSet<City> Cities { get; set; }
        DbSet<BeerServedInPubs> BeerServedInPubs { get; set; }
        DbSet<Brewery> Breweries { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<PubAdmin> PubAdmins { get; set; }
        DbSet<BreweryAdmin> BreweryAdmins { get; set; }
        DbSet<BrewerySubstitution> BrewerySubstitutions { get; set; }
    }

    public class DataContext : DbContext, IDataContext
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
            modelBuilder.Entity<BeerServedInPubs>()
                .Property(s => s.Tap)
                .HasDefaultValue(1);
            modelBuilder.Entity<BeerServedInPubs>()
                .Property(s => s.Volume)
                .HasDefaultValue(0.5m);
            modelBuilder.Entity<BreweryAdmin>()
                .HasOne(ba => ba.User)
                .WithMany(u => u.BreweryAdmins)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BreweryAdmin>()
                .HasOne(ba => ba.Brewery)
                .WithMany(b => b.Admins)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BrewerySubstitution>()
                .HasOne(ba => ba.Brewery)
                .WithMany(b => b.Substitutions)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PubAdmin>()
                .HasOne(ba => ba.User)
                .WithMany(u => u.PubAdmins)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PubAdmin>()
                .HasOne(ba => ba.Pub)
                .WithMany(b => b.Admins)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Beer>()
                .Property(b => b.ServeKind)
                .HasDefaultValue(Beer.Serve.OnTap);

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
                Password = UserBase.GetHash(configuration.GetValue<string>("DefaultAdminPassword"))
            };

            Users.Add(admin);
            SaveChanges();
        }

        private void EnsurePubData()
        {
            if (!Breweries.Any())
            {
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
            if (Countries.Count() == 1)
            {
                Countries.AddRange(
                    new Country { Id = "AF", Name = "Afghanistan" },
                    new Country { Id = "AL", Name = "Albania" },
                    new Country { Id = "DZ", Name = "Algeria" },
                    new Country { Id = "AS", Name = "American Samoa" },
                    new Country { Id = "AD", Name = "Andorra" },
                    new Country { Id = "AO", Name = "Angola" },
                    new Country { Id = "AI", Name = "Anguilla" },
                    new Country { Id = "AQ", Name = "Antarctica" },
                    new Country { Id = "AG", Name = "Antigua and Barbuda" },
                    new Country { Id = "AR", Name = "Argentina" },
                    new Country { Id = "AM", Name = "Armenia" },
                    new Country { Id = "AW", Name = "Aruba" },
                    new Country { Id = "AU", Name = "Australia" },
                    new Country { Id = "AT", Name = "Austria" },
                    new Country { Id = "AZ", Name = "Azerbaijan" },
                    new Country { Id = "BS", Name = "Bahamas" },
                    new Country { Id = "BH", Name = "Bahrain" },
                    new Country { Id = "BD", Name = "Bangladesh" },
                    new Country { Id = "BB", Name = "Barbados" },
                    new Country { Id = "BY", Name = "Belarus" },
                    new Country { Id = "BE", Name = "Belgium" },
                    new Country { Id = "BZ", Name = "Belize" },
                    new Country { Id = "BJ", Name = "Benin" },
                    new Country { Id = "BM", Name = "Bermuda" },
                    new Country { Id = "BT", Name = "Bhutan" },
                    new Country { Id = "BO", Name = "Bolivia" },
                    new Country { Id = "BQ", Name = "Bonaire" },
                    new Country { Id = "BA", Name = "Bosnia and Herzegovina" },
                    new Country { Id = "BW", Name = "Botswana" },
                    new Country { Id = "BV", Name = "Bouvet Island" },
                    new Country { Id = "BR", Name = "Brazil" },
                    new Country { Id = "IO", Name = "British Indian Ocean Territory" },
                    new Country { Id = "BN", Name = "Brunei Darussalam" },
                    new Country { Id = "BG", Name = "Bulgaria" },
                    new Country { Id = "BF", Name = "Burkina Faso" },
                    new Country { Id = "BI", Name = "Burundi" },
                    new Country { Id = "KH", Name = "Cambodia" },
                    new Country { Id = "CM", Name = "Cameroon" },
                    new Country { Id = "CA", Name = "Canada" },
                    new Country { Id = "CV", Name = "Cape Verde" },
                    new Country { Id = "KY", Name = "Cayman Islands" },
                    new Country { Id = "CF", Name = "Central African Republic" },
                    new Country { Id = "TD", Name = "Chad" },
                    new Country { Id = "CL", Name = "Chile" },
                    new Country { Id = "CN", Name = "China" },
                    new Country { Id = "CX", Name = "Christmas Island" },
                    new Country { Id = "CC", Name = "Cocos (Keeling) Islands" },
                    new Country { Id = "CO", Name = "Colombia" },
                    new Country { Id = "KM", Name = "Comoros" },
                    new Country { Id = "CG", Name = "Congo" },
                    new Country { Id = "CD", Name = "Democratic Republic of the Congo" },
                    new Country { Id = "CK", Name = "Cook Islands" },
                    new Country { Id = "CR", Name = "Costa Rica" },
                    new Country { Id = "HR", Name = "Croatia" },
                    new Country { Id = "CU", Name = "Cuba" },
                    new Country { Id = "CW", Name = "CuraÃ§ao" },
                    new Country { Id = "CY", Name = "Cyprus" },
                    new Country { Id = "CZ", Name = "Czech Republic" },
                    new Country { Id = "CI", Name = "CÃ´te d'Ivoire" },
                    new Country { Id = "DK", Name = "Denmark" },
                    new Country { Id = "DJ", Name = "Djibouti" },
                    new Country { Id = "DM", Name = "Dominica" },
                    new Country { Id = "DO", Name = "Dominican Republic" },
                    new Country { Id = "EC", Name = "Ecuador" },
                    new Country { Id = "EG", Name = "Egypt" },
                    new Country { Id = "SV", Name = "El Salvador" },
                    new Country { Id = "GQ", Name = "Equatorial Guinea" },
                    new Country { Id = "ER", Name = "Eritrea" },
                    new Country { Id = "EE", Name = "Estonia" },
                    new Country { Id = "ET", Name = "Ethiopia" },
                    new Country { Id = "FK", Name = "Falkland Islands (Malvinas)" },
                    new Country { Id = "FO", Name = "Faroe Islands" },
                    new Country { Id = "FJ", Name = "Fiji" },
                    new Country { Id = "FI", Name = "Finland" },
                    new Country { Id = "FR", Name = "France" },
                    new Country { Id = "GF", Name = "French Guiana" },
                    new Country { Id = "PF", Name = "French Polynesia" },
                    new Country { Id = "TF", Name = "French Southern Territories" },
                    new Country { Id = "GA", Name = "Gabon" },
                    new Country { Id = "GM", Name = "Gambia" },
                    new Country { Id = "GE", Name = "Georgia" },
                    new Country { Id = "DE", Name = "Germany" },
                    new Country { Id = "GH", Name = "Ghana" },
                    new Country { Id = "GI", Name = "Gibraltar" },
                    new Country { Id = "GR", Name = "Greece" },
                    new Country { Id = "GL", Name = "Greenland" },
                    new Country { Id = "GD", Name = "Grenada" },
                    new Country { Id = "GP", Name = "Guadeloupe" },
                    new Country { Id = "GU", Name = "Guam" },
                    new Country { Id = "GT", Name = "Guatemala" },
                    new Country { Id = "GG", Name = "Guernsey" },
                    new Country { Id = "GN", Name = "Guinea" },
                    new Country { Id = "GW", Name = "Guinea-Bissau" },
                    new Country { Id = "GY", Name = "Guyana" },
                    new Country { Id = "HT", Name = "Haiti" },
                    new Country { Id = "HM", Name = "Heard Island and McDonald Mcdonald Islands" },
                    new Country { Id = "VA", Name = "Holy See (Vatican City State)" },
                    new Country { Id = "HN", Name = "Honduras" },
                    new Country { Id = "HK", Name = "Hong Kong" },
                    new Country { Id = "HU", Name = "Hungary" },
                    new Country { Id = "IS", Name = "Iceland" },
                    new Country { Id = "IN", Name = "India" },
                    new Country { Id = "ID", Name = "Indonesia" },
                    new Country { Id = "IR", Name = "Iran, Islamic Republic of" },
                    new Country { Id = "IQ", Name = "Iraq" },
                    new Country { Id = "IE", Name = "Ireland" },
                    new Country { Id = "IM", Name = "Isle of Man" },
                    new Country { Id = "IL", Name = "Israel" },
                    new Country { Id = "IT", Name = "Italy" },
                    new Country { Id = "JM", Name = "Jamaica" },
                    new Country { Id = "JP", Name = "Japan" },
                    new Country { Id = "JE", Name = "Jersey" },
                    new Country { Id = "JO", Name = "Jordan" },
                    new Country { Id = "KZ", Name = "Kazakhstan" },
                    new Country { Id = "KE", Name = "Kenya" },
                    new Country { Id = "KI", Name = "Kiribati" },
                    new Country { Id = "KP", Name = "Korea, Democratic People's Republic of" },
                    new Country { Id = "KR", Name = "Korea, Republic of" },
                    new Country { Id = "KW", Name = "Kuwait" },
                    new Country { Id = "KG", Name = "Kyrgyzstan" },
                    new Country { Id = "LA", Name = "Lao People's Democratic Republic" },
                    new Country { Id = "LV", Name = "Latvia" },
                    new Country { Id = "LB", Name = "Lebanon" },
                    new Country { Id = "LS", Name = "Lesotho" },
                    new Country { Id = "LR", Name = "Liberia" },
                    new Country { Id = "LY", Name = "Libya" },
                    new Country { Id = "LI", Name = "Liechtenstein" },
                    new Country { Id = "LT", Name = "Lithuania" },
                    new Country { Id = "LU", Name = "Luxembourg" },
                    new Country { Id = "MO", Name = "Macao" },
                    new Country { Id = "MK", Name = "Macedonia, the Former Yugoslav Republic of" },
                    new Country { Id = "MG", Name = "Madagascar" },
                    new Country { Id = "MW", Name = "Malawi" },
                    new Country { Id = "MY", Name = "Malaysia" },
                    new Country { Id = "MV", Name = "Maldives" },
                    new Country { Id = "ML", Name = "Mali" },
                    new Country { Id = "MT", Name = "Malta" },
                    new Country { Id = "MH", Name = "Marshall Islands" },
                    new Country { Id = "MQ", Name = "Martinique" },
                    new Country { Id = "MR", Name = "Mauritania" },
                    new Country { Id = "MU", Name = "Mauritius" },
                    new Country { Id = "YT", Name = "Mayotte" },
                    new Country { Id = "MX", Name = "Mexico" },
                    new Country { Id = "FM", Name = "Micronesia, Federated States of" },
                    new Country { Id = "MD", Name = "Moldova, Republic of" },
                    new Country { Id = "MC", Name = "Monaco" },
                    new Country { Id = "MN", Name = "Mongolia" },
                    new Country { Id = "ME", Name = "Montenegro" },
                    new Country { Id = "MS", Name = "Montserrat" },
                    new Country { Id = "MA", Name = "Morocco" },
                    new Country { Id = "MZ", Name = "Mozambique" },
                    new Country { Id = "MM", Name = "Myanmar" },
                    new Country { Id = "NA", Name = "Namibia" },
                    new Country { Id = "NR", Name = "Nauru" },
                    new Country { Id = "NP", Name = "Nepal" },
                    new Country { Id = "NL", Name = "Netherlands" },
                    new Country { Id = "NC", Name = "New Caledonia" },
                    new Country { Id = "NZ", Name = "New Zealand" },
                    new Country { Id = "NI", Name = "Nicaragua" },
                    new Country { Id = "NE", Name = "Niger" },
                    new Country { Id = "NG", Name = "Nigeria" },
                    new Country { Id = "NU", Name = "Niue" },
                    new Country { Id = "NF", Name = "Norfolk Island" },
                    new Country { Id = "MP", Name = "Northern Mariana Islands" },
                    new Country { Id = "NO", Name = "Norway" },
                    new Country { Id = "OM", Name = "Oman" },
                    new Country { Id = "PK", Name = "Pakistan" },
                    new Country { Id = "PW", Name = "Palau" },
                    new Country { Id = "PS", Name = "Palestine, State of" },
                    new Country { Id = "PA", Name = "Panama" },
                    new Country { Id = "PG", Name = "Papua New Guinea" },
                    new Country { Id = "PY", Name = "Paraguay" },
                    new Country { Id = "PE", Name = "Peru" },
                    new Country { Id = "PH", Name = "Philippines" },
                    new Country { Id = "PN", Name = "Pitcairn" },
                    new Country { Id = "PL", Name = "Poland" },
                    new Country { Id = "PT", Name = "Portugal" },
                    new Country { Id = "PR", Name = "Puerto Rico" },
                    new Country { Id = "QA", Name = "Qatar" },
                    new Country { Id = "RO", Name = "Romania" },
                    new Country { Id = "RU", Name = "Russian Federation" },
                    new Country { Id = "RW", Name = "Rwanda" },
                    new Country { Id = "RE", Name = "Reunion" },
                    new Country { Id = "BL", Name = "Saint Barthelemy" },
                    new Country { Id = "SH", Name = "Saint Helena" },
                    new Country { Id = "KN", Name = "Saint Kitts and Nevis" },
                    new Country { Id = "LC", Name = "Saint Lucia" },
                    new Country { Id = "MF", Name = "Saint Martin (French part)" },
                    new Country { Id = "PM", Name = "Saint Pierre and Miquelon" },
                    new Country { Id = "VC", Name = "Saint Vincent and the Grenadines" },
                    new Country { Id = "WS", Name = "Samoa" },
                    new Country { Id = "SM", Name = "San Marino" },
                    new Country { Id = "ST", Name = "Sao Tome and Principe" },
                    new Country { Id = "SA", Name = "Saudi Arabia" },
                    new Country { Id = "SN", Name = "Senegal" },
                    new Country { Id = "RS", Name = "Serbia" },
                    new Country { Id = "SC", Name = "Seychelles" },
                    new Country { Id = "SL", Name = "Sierra Leone" },
                    new Country { Id = "SG", Name = "Singapore" },
                    new Country { Id = "SX", Name = "Sint Maarten (Dutch part)" },
                    new Country { Id = "SK", Name = "Slovakia" },
                    new Country { Id = "SI", Name = "Slovenia" },
                    new Country { Id = "SB", Name = "Solomon Islands" },
                    new Country { Id = "SO", Name = "Somalia" },
                    new Country { Id = "ZA", Name = "South Africa" },
                    new Country { Id = "GS", Name = "South Georgia and the South Sandwich Islands" },
                    new Country { Id = "SS", Name = "South Sudan" },
                    new Country { Id = "ES", Name = "Spain" },
                    new Country { Id = "LK", Name = "Sri Lanka" },
                    new Country { Id = "SD", Name = "Sudan" },
                    new Country { Id = "SR", Name = "Suriname" },
                    new Country { Id = "SJ", Name = "Svalbard and Jan Mayen" },
                    new Country { Id = "SZ", Name = "Swaziland" },
                    new Country { Id = "SE", Name = "Sweden" },
                    new Country { Id = "CH", Name = "Switzerland" },
                    new Country { Id = "SY", Name = "Syrian Arab Republic" },
                    new Country { Id = "TW", Name = "Taiwan, Province of China" },
                    new Country { Id = "TJ", Name = "Tajikistan" },
                    new Country { Id = "TZ", Name = "United Republic of Tanzania" },
                    new Country { Id = "TH", Name = "Thailand" },
                    new Country { Id = "TL", Name = "Timor-Leste" },
                    new Country { Id = "TG", Name = "Togo" },
                    new Country { Id = "TK", Name = "Tokelau" },
                    new Country { Id = "TO", Name = "Tonga" },
                    new Country { Id = "TT", Name = "Trinidad and Tobago" },
                    new Country { Id = "TN", Name = "Tunisia" },
                    new Country { Id = "TR", Name = "Turkey" },
                    new Country { Id = "TM", Name = "Turkmenistan" },
                    new Country { Id = "TC", Name = "Turks and Caicos Islands" },
                    new Country { Id = "TV", Name = "Tuvalu" },
                    new Country { Id = "UG", Name = "Uganda" },
                    new Country { Id = "AE", Name = "United Arab Emirates" },
                    new Country { Id = "GB", Name = "United Kingdom" },
                    new Country { Id = "US", Name = "United States" },
                    new Country { Id = "UM", Name = "United States Minor Outlying Islands" },
                    new Country { Id = "UY", Name = "Uruguay" },
                    new Country { Id = "UZ", Name = "Uzbekistan" },
                    new Country { Id = "VU", Name = "Vanuatu" },
                    new Country { Id = "VE", Name = "Venezuela" },
                    new Country { Id = "VN", Name = "Viet Nam" },
                    new Country { Id = "VG", Name = "British Virgin Islands" },
                    new Country { Id = "VI", Name = "US Virgin Islands" },
                    new Country { Id = "WF", Name = "Wallis and Futuna" },
                    new Country { Id = "EH", Name = "Western Sahara" },
                    new Country { Id = "YE", Name = "Yemen" },
                    new Country { Id = "ZM", Name = "Zambia" },
                    new Country { Id = "ZW", Name = "Zimbabwe" },
                    new Country { Id = "AX", Name = "Aland Islands" }
                    );
                SaveChanges();
            }
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
        public DbSet<BrewerySubstitution> BrewerySubstitutions { get; set; }
    }
}