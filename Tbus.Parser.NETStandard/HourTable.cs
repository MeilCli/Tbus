using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tbus.Parser.NETStandard
{
    public class HourTable
    {
        [JsonProperty("indexed_hours")]
        public List<IndexedHour> IndexedHours { get; set; }
    }
}
