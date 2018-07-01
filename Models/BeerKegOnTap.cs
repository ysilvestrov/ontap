using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ontap.Models
{
    public class BeerKegOnTap
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public BeerKeg Keg { get; set; }
        public Tap Tap { get; set; }
        public int Priority { get; set; }
        public DateTime? InstallTime { get; set; }
        public DateTime? DeinstallTime { get; set; }
    }
}