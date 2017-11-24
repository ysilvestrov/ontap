using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ontap.Models
{
    public class Pub
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public virtual City City { get; set; }
        [JsonProperty("serves")]
        public virtual IList<BeerServedInPubs> BeerServedInPubs { get; set; }
        public ICollection<PubAdmin> Admins { get; set; }
        public string Image { get; set; }
        public string TaplistHeaderImage { get; set; }
        public string TaplistFooterImage { get; set; }
        public string FacebookUrl { get; set; }
        public string VkontakteUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string BookingUrl { get; set; }
        public string ParserOptions { get; set; }
        public int TapNumber { get; set; } = 0;
    }
}
