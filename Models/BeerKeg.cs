using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ontap.Models
{
    public class BeerKeg
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Keg Keg { get; set; }
        public Beer Beer { get; set; }
        public Brewery Owner { get; set; }
        public Pub Buyer { get; set; }
        public KegStatus Status { get; set; }
        public DateTime BrewingDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime InstallationDate { get; set; }
        public DateTime DeinstallationDate { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public IList<BeerKegWeight> Weights { get; set; }
        public IList<BeerKegOnTap> BeerKegsOnTap { get; set; }
    }
}