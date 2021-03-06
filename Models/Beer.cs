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

        public enum Serve
        {
            OnTap,
            Bottle
        }

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Type { get; set; }
        public decimal Alcohol { get; set; }
        public decimal Gravity { get; set; }
        public decimal Ibu { get; set; }
        public Classification Kind { get; set; } = Classification.Lager;
        public string BjcpStyle { get; set; } = "";
        public string Description { get; set; }

        [NotMapped]
        public string[] Labels
        {
            get => (_labels ?? "").Split('|');
            set => _labels = string.Join("|", value);
        }

        public string _labels { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual IList<BeerPrice> BeerPrices { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual IList<BeerKeg> BeerKegs { get; set; }

        public virtual Brewery Brewery { get; set; }
        public string Image { get; set; }
        public Serve ServeKind { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public ICollection<BeerSubstitution> Substitutions { get; set; }
    }
}
