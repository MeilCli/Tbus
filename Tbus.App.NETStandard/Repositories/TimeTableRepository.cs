using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Tbus.App.NETStandard.Constants;
using Tbus.Parser.NETStandard;
using Tbus.Calendar.NETStandard;

namespace Tbus.App.NETStandard.Repositories
{
    internal class TimeTableRepository : ITimeTableRepository
    {
        private readonly ICalendar calendar;

        public TimeTableRepository()
        {
            calendar = new JapaneseCalendar();
        }

        public TimeTableRepository(ICalendar calendar)
        {
            this.calendar = calendar;
        }

        public async Task<List<(string id, DayTable dayTable, HourTable hourTable)>> GetTodayTimeTablesAsync()
        {
            var result = new List<(string, DayTable, HourTable)>();
            List<TimeTable> timeTables = await getDefaultTimeTablesAsync();
            await Task.Run(() =>
            {
                foreach (var timeTableGroup in timeTables.GroupBy(x => x.Id))
                {
                    (string, DayTable, HourTable) todayTimeTableData = getTodayTimeTableData(timeTableGroup);
                    if (todayTimeTableData != default)
                    {
                        result.Add(todayTimeTableData);
                    }
                }
            });
            return result;
        }

        private (string id, DayTable dayTable, HourTable hourTable) getTodayTimeTableData(
            IGrouping<string, TimeTable> timeTables)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
            DayType todayType = calendar.JudgeDayType(today);
            (string, DayTable, HourTable) specialDayData = timeTables.Where(x => x.SpecialDays != null)
                .SelectMany(x => x.SpecialDays.Select(y => new { id = x.Id, date = y.Key, dayTable = y.Value, hourTable = x.HourTable }))
                .Where(x => x.date.Year == today.Year)
                .Where(x => x.date.Month == today.Month)
                .Where(x => x.date.Day == today.Day)
                .Select(x => (x.id, x.dayTable, x.hourTable))
                .FirstOrDefault();
            if (specialDayData != default)
            {
                return specialDayData;
            }

            (string, DayTable, HourTable) limitedDayData = timeTables.Where(x => x.LimitedTimeOption != null)
                .Where(x => x.LimitedTimeOption.StartDay <= today)
                .Where(x => today <= x.LimitedTimeOption.EndDay)
                .Where(x => x.WeekdayTable != null)
                .Where(x => x.SaturdayTable != null)
                .Where(x => x.SundayTable != null)
                .Select(x => selectTodayData(x, todayType))
                .FirstOrDefault();
            if (limitedDayData != default)
            {
                return limitedDayData;
            }

            (string, DayTable, HourTable) todayData = timeTables.Where(x => x.LimitedTimeOption == null)
                .Where(x => x.WeekdayTable != null)
                .Where(x => x.SaturdayTable != null)
                .Where(x => x.SundayTable != null)
                .Select(x => selectTodayData(x, todayType))
                .FirstOrDefault();
            if (todayData != default)
            {
                return todayData;
            }

            return default;
        }

        private (string, DayTable, HourTable) selectTodayData(TimeTable timeTable, DayType todayType)
        {
            DayTable dayTable;
            switch (todayType)
            {
                case DayType.Weekday:
                    dayTable = timeTable.WeekdayTable;
                    break;
                case DayType.Saturday:
                    dayTable = timeTable.SaturdayTable;
                    break;
                default:
                    dayTable = timeTable.SundayTable;
                    break;
            }
            return (timeTable.Id, dayTable, timeTable.HourTable);
        }

        private async Task<List<TimeTable>> getDefaultTimeTablesAsync()
        {
            var result = new List<TimeTable>();

            async Task addAllAsync(string[] fileNames)
            {
                foreach (string fileName in fileNames)
                {
                    result.Add(await getDefaultTimeTableAsync(fileName));
                }
            }

            await addAllAsync(TimeTableFileConstant.KansaiToTakatuki);
            await addAllAsync(TimeTableFileConstant.KansaiToTonda);
            await addAllAsync(TimeTableFileConstant.TakatukiToKansai);
            await addAllAsync(TimeTableFileConstant.TondaToKansai);

            return result;
        }

        private async Task<TimeTable> getDefaultTimeTableAsync(string fileName)
        {
            Assembly assenbly = typeof(TimeTableRepository).Assembly;
            using (var stream = assenbly.GetManifestResourceStream($"Tbus.App.NETStandard.Resources.{fileName}")
                ?? throw new Exception("not found defult file"))
            using (var streamReader = new StreamReader(stream))
            {
                string jsonText = await streamReader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<TimeTable>(jsonText);
            }
        }
    }
}
