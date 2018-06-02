using Newtonsoft.Json;

namespace Tbus.Parser.NETStandard
{
    public class Bus
    {
        [JsonProperty("hour")]
        public int Hour { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }
}
