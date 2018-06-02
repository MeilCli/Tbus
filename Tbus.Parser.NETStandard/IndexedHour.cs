using Newtonsoft.Json;

namespace Tbus.Parser.NETStandard
{
    public class IndexedHour
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("hour")]
        public int Hour { get; set; }

        public IndexedHour() { }

        public IndexedHour(int index, int hour)
        {
            Index = index;
            Hour = hour;
        }
    }
}
