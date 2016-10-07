using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Ontap.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool CanAdminPub { get; set; } = false;
        public bool CanAdminBrewery { get; set; } = false;
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<PubAdmin> PubAdmins { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<BreweryAdmin> BreweryAdmins { get; set; }
        [NotMapped]
        public IEnumerable<Pub> Pubs => PubAdmins != null && PubAdmins.Any() ? PubAdmins.Select(pa => pa.Pub).Distinct() : new Pub[0];

        [NotMapped]
        public IEnumerable<Brewery> Breweries
            => BreweryAdmins != null && BreweryAdmins.Any() ? BreweryAdmins.Select(ba => ba.Brewery).Distinct() : new Brewery[0];
    }
}
