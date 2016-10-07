using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ontap.Models
{
    public class Brewery
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Country Country { get; set; }
        public virtual IList<Beer> Beers { get; set; }
        public IEnumerable<BreweryAdmin> Admins { get; set; }
    }
}
