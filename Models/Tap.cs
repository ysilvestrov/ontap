using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ontap.Models
{
    public enum TapStatus
    {
        Working = 2,
        Problem = 1,
        Free = 0
    }

    public class Tap
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Pub Pub { get; set; }
        public string Number { get; set; }
        public bool HasHopinator { get; set; }
        public string Fitting { get; set; }
        public int NitrogenPercentage { get; set; }
        public TapStatus Status { get; set; }
        public IList<BeerKegOnTap> BeerKegsOnTap { get; set; }
    }
}