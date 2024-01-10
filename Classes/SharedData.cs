namespace cs_web_voting.Singletons
{
    public static class SharedData
    {
        public static List<string> NominatedMaps { get; } = new List<string>();
        public static List<object> VotedMaps { get; } = new List<object>();
        public static string JsonData { get; set; } = string.Empty;
        public static Dictionary<string, RoomData> countdowns = new Dictionary<string, RoomData>();
        
        
    }
    // Your MyObject class
    public class VoteObject
    {
        [System.Text.Json.Serialization.JsonPropertyName("Name")]
        public string? Name { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("Count")]
        public int Count { get; set; }
        // Add other fields as needed
    }
    public class RoomData
    {
        public int Mode { get; set; }
        public int Countdown { get; set; }
    }
}