using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Ontap.Models
{
    public class BeerPrice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Beer Beer { get; set; }
        public virtual Pub Pub { get; set; }
        public decimal Price { get; set; }
        [DefaultValue(typeof(decimal), "0.5")]
        public decimal Volume { get; set; } = 0.5m;
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime Updated { get; set; }
    }
}