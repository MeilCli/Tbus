using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tbus.Parser.NETStandard
{
    public class TimeTable
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("limited_time_option")]
        public LimitedTimeOption LimitedTimeOption { get; set; }

        [JsonProperty("weekday_table")]
        public DayTable WeekdayTable { get; set; }

        [JsonProperty("saturday_table")]
        public DayTable SaturdayTable { get; set; }

        [JsonProperty("sunday_table")]
        public DayTable SundayTable { get; set; }

        [JsonProperty("special_days")]
        public Dictionary<DateTime, DayTable> SpecialDays { get; set; }

        [JsonProperty("hour_table")]
        public HourTable HourTable { get; set; }
    }
}
