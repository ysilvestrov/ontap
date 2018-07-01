using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ontap.Models
{
    public enum KegStatus
    {
        Waiting,
        OnTap,
        Empty,
        Problematic
    }

    public class Keg
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public char Fitting { get; set; }
        public decimal Volume { get; set; }
        public bool IsReturnable { get; set; }
        public string Material { get; set; }
        public decimal EmptyWeight { get; set; }
        public IList<BeerKeg> BeerKegs { get; set; }
    }
}
