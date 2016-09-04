using System.ComponentModel.DataAnnotations;

namespace Ontap.Models
{
    public class City
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
