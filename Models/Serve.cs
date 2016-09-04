using System.ComponentModel.DataAnnotations.Schema;

namespace Ontap.Models
{
    [NotMapped]
    public class Serve
    {
        public Beer Beer { get; set; }
        public decimal Price { get; set; }
    }
}