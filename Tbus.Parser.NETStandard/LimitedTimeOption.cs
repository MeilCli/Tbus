using Newtonsoft.Json;
using System;

namespace Tbus.Parser.NETStandard
{
    /// <summary>
    /// StartDay <= n day <= EndDay
    /// </summary>
    public class LimitedTimeOption
    {
        [JsonProperty("start_day")]
        public DateTime StartDay { get; set; }

        [JsonProperty("end_day")]
        public DateTime EndDay { get; set; }
    }
}
