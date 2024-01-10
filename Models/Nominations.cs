namespace cs_web_voting.Models
{
    public class Nominations
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string? Name { get; set; }
    }
}