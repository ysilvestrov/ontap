namespace Ontap.Models
{
    public class PubAdmin
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Pub Pub { get; set; }
    }
}