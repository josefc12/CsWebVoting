namespace cs_web_voting.Models
{
    public class Voters
    {
        public int ID { get; set; }
        public int SessionID { get; set; }
        public string? ConnectionID { get; set; }
        public string? Name { get; set; }
        public int Admin { get; set; }
        public int VtAmnt { get; set; }
        public int NmntAmnt { get; set; }
    }
}