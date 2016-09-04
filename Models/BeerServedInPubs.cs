namespace Ontap.Models
{
    public class BeerServedInPubs
    {
        public int Id { get; set; }
        public virtual Beer Served { get; set; }
        public virtual Pub ServedIn { get; set; }
        public decimal Price { get; set; }
    }
}