using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ontap.Models
{
    public class BeerKegWeight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public BeerKeg Keg { get; set; }
        public DateTime Date { get; set; }
        public decimal Weight { get; set; }
    }
}