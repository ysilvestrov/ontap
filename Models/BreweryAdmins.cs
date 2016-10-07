namespace Ontap.Models
{
    public class BreweryAdmin
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Brewery Brewery { get; set; }
    }
}