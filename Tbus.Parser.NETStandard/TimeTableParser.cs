using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Tbus.Parser.NETStandard
{
    public class TimeTableParser
    {
        private const string defaultDestinationQuery = "td.td-destination";
        private const string tableQuery = "table.diagram-table";
        private const string hourTableQuery = "tr.l2";
        private const string hourQuery = "th.hour";
        private const string specialdayDateQuery = "th.temporary-diagram-date-label-entry";
        private const string weekdayQuery = "td.wkd";
        private const string saturdayQuery = "td.std";
        private const string sundayQuery = "td.snd";
        private const string specialdayQuery = "td.temporary-wkd";
        private const string busQuery = "div.diagram-item";
        private const string minuteQuery = "div.mm a";
        private const string destinationQuery = "span.speech-only";

        public async Task<TimeTable> ParseUrlAsync(string url, string id, LimitedTimeOption limitedTimeOption = null)
        {
            IDocument document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(url);
            return parse(document, id, limitedTimeOption);
        }

        public async Task<TimeTable> ParseHtmlAsync(string html, string id, LimitedTimeOption limitedTimeOption = null)
        {
            var parser = new HtmlParser();
            IDocument document = await parser.ParseAsync(html);
            return parse(document, id, limitedTimeOption);
        }

        private TimeTable parse(IDocument document, string id, LimitedTimeOption limitedTimeOption = null)
        {
            IEnumerable<IElement> destinations = document.QuerySelectorAll(defaultDestinationQuery);
            string defaultDestination = destinations != null ? string.Join(" または ", destinations.Select(x => x.InnerHtml.Trim())) : null;

            IHtmlCollection<IElement> htmlCollection = document.QuerySelectorAll(tableQuery);
            var hourTable = new HourTable
            {
                IndexedHours = new List<IndexedHour>()
            };
            var weekdayTable = new DayTable
            {
                Buses = new List<Bus>()
            };
            var saturdayTable = new DayTable
            {
                Buses = new List<Bus>()
            };
            var sundayTable = new DayTable
            {
                Buses = new List<Bus>()
            };

            if (htmlCollection.Length < 1)
            {
                throw new TbusParserException("not found diagram-table");
            }
            if (htmlCollection[0] == null)
            {
                throw new TbusParserException("not found table");
            }

            var specialDays = new List<(DateTime day, DayTable dayTable)>();
            foreach (var element in htmlCollection[0].QuerySelectorAll(specialdayDateQuery))
            {
                string specialDayText = element?.InnerHtml
                    ?? throw new TbusParserException("not found special day");
                if (tryParseDate(specialDayText, out DateTime specialDay))
                {
                    specialDays.Add((specialDay, new DayTable { Buses = new List<Bus>() }));
                }
                else
                {
                    throw new TbusParserException("cannot parse special day");
                }
            }

            int index = 0;
            foreach (var element in htmlCollection[0].QuerySelectorAll(hourTableQuery))
            {
                string hourText = element.QuerySelector(hourQuery)?.InnerHtml
                    ?? throw new TbusParserException("not found hour");
                if (tryParseInt(hourText, out int hour))
                {
                    hourTable.IndexedHours.Add(new IndexedHour(index, hour));
                }
                else
                {
                    throw new TbusParserException("cannot parse hour");
                }

                parseDayTableInHour(element.QuerySelector(weekdayQuery), hour, weekdayTable, defaultDestination);
                parseDayTableInHour(element.QuerySelector(saturdayQuery), hour, saturdayTable, defaultDestination);
                parseDayTableInHour(element.QuerySelector(sundayQuery), hour, sundayTable, defaultDestination);
                int specialDayIndex = 0;
                foreach (var e in element.QuerySelectorAll(specialdayQuery))
                {
                    if (specialDays.Count <= specialDayIndex)
                    {
                        throw new TbusParserException("out range special days");
                    }
                    parseDayTableInHour(e, hour, specialDays[specialDayIndex].dayTable, defaultDestination);
                    specialDayIndex++;
                }

                index++;
            }

            var timeTable = new TimeTable()
            {
                Id = id,
                CreatedAt = DateTime.Now,
                LimitedTimeOption = limitedTimeOption,
                HourTable = hourTable,
                WeekdayTable = weekdayTable,
                SaturdayTable = saturdayTable,
                SundayTable = sundayTable,
                SpecialDays = specialDays.ToDictionary(x => x.day, x => x.dayTable)
            };
            return timeTable;
        }

        private void parseDayTableInHour(IElement parent, int hour, DayTable dayTable, string defaultDestination)
        {
            if (parent == null)
            {
                return;
            }
            if (parent.HasChildNodes == false)
            {
                throw new TbusParserException("not found day table");
            }
            foreach (var element in parent.QuerySelectorAll(busQuery))
            {
                string minuteText = element.QuerySelector(minuteQuery)?.InnerHtml
                    ?? throw new TbusParserException("not found minute");
                string destination = element.QuerySelector(destinationQuery)?.InnerHtml ?? defaultDestination
                    ?? throw new TbusParserException("not found destination");

                if (tryParseInt(minuteText, out int minute))
                {
                    dayTable.Buses.Add(
                        new Bus()
                        {
                            Hour = hour,
                            Minute = minute,
                            Destination = destination.Trim()
                        }
                    );
                }
                else
                {
                    throw new TbusParserException("cannot parse minute");
                }
            }
        }

        private bool tryParseInt(string value, out int result)
        {
            return int.TryParse(value.Trim(), out result);
        }

        private bool tryParseDate(string value, out DateTime date)
        {
            try
            {
                date = DateTime.Parse(value.Trim(), CultureInfo.CreateSpecificCulture("ja-JP"), DateTimeStyles.AssumeLocal);
                return true;
            }
            catch (Exception)
            {
                date = default;
                return false;
            }
        }
    }
}
