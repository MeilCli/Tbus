using System;
using System.Collections.Generic;

namespace Tbus.Calendar.NETStandard
{
    public class JapaneseCalendar : ICalendar
    {
        public List<(DateTime date, DayType dayType)> GetDayTypesOfYear(int year)
        {
            var result = new List<(DateTime, DayType)>();
            var day = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local);
            while (day.Year == year)
            {
                result.Add((day, JudgeDayType(day)));
                day = day.AddDays(1);
            }
            return result;
        }

        public List<DateTime> GetHolidaysOfYear(int year)
        {
            var result = new List<DateTime>();
            var day = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local);
            while (day.Year == year)
            {
                if (isHoliday(day))
                {
                    result.Add(day);
                }
                day = day.AddDays(1);
            }
            return result;
        }

        public DayType JudgeDayType(DateTime date)
        {
            if (isHoliday(date))
            {
                return DayType.Holiday;
            }
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return DayType.Sunday;
            }
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return DayType.Saturday;
            }
            return DayType.Weekday;
        }

        /// <summary>
        /// http://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool isHoliday(DateTime date)
        {
            if (isNormalHoliday(date))
            {
                return true;
            }
            if (isSubstituteHoliday(date))
            {
                return true;
            }
            if (isNationalHoliday(date))
            {
                return true;
            }

            return false;
        }

        private bool isNormalHoliday(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            DayOfWeek dayOfWeek = date.DayOfWeek;
            int weekOfMonth = (date.Day - (int)dayOfWeek) / 7 + 1;


            (int month, int day) monthAndDay = (month, day);
            if (monthAndDay == (1, 1) || monthAndDay == (2, 11) || monthAndDay == (4, 29)
                || monthAndDay == (5, 3) || monthAndDay == (5, 4) || monthAndDay == (5, 5)
                || monthAndDay == (8, 11) || monthAndDay == (11, 3) || monthAndDay == (11, 23))
            {
                return true;
            }

            // 天皇誕生日は変わる
            if (year == 2018 && monthAndDay == (12, 23))
            {
                return true;
            }
            if (2020 <= year && monthAndDay == (2, 23))
            {
                return true;
            }

            if (month == 1 && weekOfMonth == 2 && dayOfWeek == DayOfWeek.Monday)
            {
                return true;
            }
            if (month == 7 && weekOfMonth == 3 && dayOfWeek == DayOfWeek.Monday)
            {
                return true;
            }
            if (month == 9 && weekOfMonth == 3 && dayOfWeek == DayOfWeek.Monday)
            {
                return true;
            }
            if (month == 10 && weekOfMonth == 2 && dayOfWeek == DayOfWeek.Monday)
            {
                return true;
            }

            {
                // 春分の日 https://ja.wikipedia.org/wiki/%E6%98%A5%E5%88%86%E3%81%AE%E6%97%A5
                // 2023年まで
                if ((year % 4 == 0 || year % 4 == 1) && month == 3 && day == 20)
                {
                    return true;
                }
                if ((year % 4 == 2 || year % 4 == 3) && month == 3 && day == 21)
                {
                    return true;
                }
            }

            {
                // 秋分の日 https://ja.wikipedia.org/wiki/%E7%A7%8B%E5%88%86%E3%81%AE%E6%97%A5
                // 2043年まで
                if (year % 4 == 0 && month == 9 && day == 22)
                {
                    return true;
                }
                if ((year % 4 == 1 || year % 4 == 2 || year % 4 == 3) && month == 9 && day == 23)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 振替休日判定
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool isSubstituteHoliday(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            int dayOfWeek = (int)date.DayOfWeek;

            for (int i = 0; i < dayOfWeek; i++)
            {
                DateTime d = date.AddDays(i - dayOfWeek);
                if (isNormalHoliday(d) == false && isNationalHoliday(d) == false)
                {
                    // 判定日の前日からその直前の日曜までに祝日がなければ振替休日ではない
                    return false;
                }
            }
            return true;
        }

        private bool isNationalHoliday(DateTime date)
        {
            // 前後の日が祝日ならば国民の祝日
            DateTime date1 = date.AddDays(-1);
            DateTime date2 = date.AddDays(1);
            if (isNormalHoliday(date1) == false && isSubstituteHoliday(date1) == false)
            {
                // 前日が振替休日の場合はあり得る
                return false;
            }
            if (isNormalHoliday(date2) == false)
            {
                return false;
            }
            return true;
        }
    }
}
