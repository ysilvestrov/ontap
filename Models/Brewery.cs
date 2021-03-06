﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ontap.Models
{
    public class Brewery
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Country Country { get; set; }
        [DefaultValue(true)]
        public bool HasOwnBeers { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public virtual IList<Beer> Beers { get; set; }
        public ICollection<BreweryAdmin> Admins { get; set; }
        public string Image { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public ICollection<BrewerySubstitution> Substitutions { get; set; }

        public IList<BeerKeg> BeerKegsOwned { get; set; }
    }
}
