using System;
using System.Collections.Generic;

namespace Tbus.Calendar.NETStandard
{
    public interface ICalendar
    {
        List<(DateTime date, DayType dayType)> GetDayTypesOfYear(int year);

        List<DateTime> GetHolidaysOfYear(int year);

        DayType JudgeDayType(DateTime date);
    }
}
