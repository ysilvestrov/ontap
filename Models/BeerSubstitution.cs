using System.ComponentModel.DataAnnotations.Schema;

namespace Ontap.Models
{
    public class BeerSubstitution
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public Beer Beer { get; set; }
    }
}