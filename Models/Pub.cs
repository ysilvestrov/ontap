using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ontap.Models
{
    public class Pub
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public virtual City City { get; set; }

        [NotMapped]
        public IEnumerable<Serve> Serves 
        {
            get
            {
                return BeerServedInPubs != null
                    ? BeerServedInPubs.Select(x => new Serve {Beer = x.Served, Price = x.Price})
                    : new Serve[0];
            }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual IList<BeerServedInPubs> BeerServedInPubs { get; set; }

        public IEnumerable<PubAdmin> Admins { get; set; }
    }
}
