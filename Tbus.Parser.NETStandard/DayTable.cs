using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tbus.Parser.NETStandard
{
    public class DayTable
    {
        [JsonProperty("buses")]
        public List<Bus> Buses { get; set; }
    }
}
