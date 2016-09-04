﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Ontap.Models
{
    public class Beer
    {
        public enum Classification
        {
            Lager,
            Ale,
            Wild
        }

        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Brewery { get; set; }
        public decimal Alcohol { get; set; }
        public decimal Gravity { get; set; }
        public decimal Ibu { get; set; }
        public Classification Kind { get; set; } = Classification.Lager;
        public string Description { get; set; }

        [NotMapped]
        public string[] Labels
        {
            get { return (_labels??"").Split('|'); } 
            set { _labels = string.Join("|", value);}
        }

        public string _labels { get; set; }

        [NotMapped]
        public IEnumerable<Pub> ServedIn
        {
            get
            {
                return BeerServedInPubs != null ? BeerServedInPubs.Select(x => x.ServedIn) : new Pub[0];
            }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual IList<BeerServedInPubs> BeerServedInPubs { get; set; }
    }
}
