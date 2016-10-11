using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    //create an extension method to override the ToString method of the DateTimeClass
    //the format should be yyyymmdd
    public static class DateTimeExt
    {
        public static string ToStringF(this DateTime val)
        {
            string year, month, day;

            year = val.Year.ToString();

            if (val.Month < 10)
            {
                month = "0" + val.Month.ToString();
            }
            else
            {
                month = val.Month.ToString();
            }

            if (val.Day < 10)
            {
                day = "0" + val.Day.ToString();
            }
            else
            {
                day = val.Day.ToString();
            }

            return year + month + day;
        }
    }

    public static class DateStringExt
    {
        public static int DaysDiff(this string s1, string s2)
        {
            DateTime d1 = new DateTime(int.Parse(s1.Substring(0, 4)), int.Parse(s1.Substring(4, 2)), int.Parse(s1.Substring(6, 2)));
            DateTime d2 = new DateTime(int.Parse(s2.Substring(0, 4)), int.Parse(s2.Substring(4, 2)), int.Parse(s2.Substring(6, 2)));

            return (int)(d1 - d2).TotalDays;
        }

        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

    }

    
}
