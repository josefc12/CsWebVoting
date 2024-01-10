using System.ComponentModel.DataAnnotations;
namespace cs_web_voting.Models

{
    public class Sessions
    {
        [Key]
        public int SessionID { get; set; }
        public int Stage { get; set; }
        public string? Name { get; set; }
    }
}