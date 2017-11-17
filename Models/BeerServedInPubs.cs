using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Ontap.Models
{
    public class BeerServedInPubs
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Beer Served { get; set; }
        public virtual Pub ServedIn { get; set; }
        public decimal Price { get; set; }
        [DefaultValue(1)]
        public int Tap { get; set; } = 1;
        [DefaultValue(typeof(decimal), "0.5")]
        public decimal Volume { get; set; } = 0.5m;
        public DateTime Updated { get; set; }
    }
}