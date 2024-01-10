namespace cs_web_voting.Models
{
    public class Votes
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string? Name { get; set; }
        public int VoteAmount { get; set; }
    }
}