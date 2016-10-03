using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class Stock
    {
        public string stockCode;
        public Dictionary<string, MarketData> marketHistory;
        public AnalyticInfo indicators;

        public Stock()
        {
            marketHistory = new Dictionary<string, MarketData>();
            indicators = new AnalyticInfo();
        }

    }

    public class AnalyticInfo
    {

        //TKey: string of date
        public Dictionary<string, decimal> SMAShort, SMALong, EMAShort, EMALong;
        public Dictionary<string, decimal> RSI, ROC, MACD, AccDist, AroonOsc;
        public int SMAPeriodShort, SMAPeriodLong, EMAPeriodShort, EMAPeriodLong;
        public int RSIPeriod, ROCPeriod, AccDistStartValue, AroonOscPeriod;
        public int MACDPeriodShort, MACDPeriodLong, MACDPeriodSignal;

        #region Constructors
        //main constructor
        public AnalyticInfo(int SMAPeriodShort, int SMAPeriodLong, int EMAPeriodShort, int EMAPeriodLong, 
                            int RSIPeriod, int ROCPeriod, int AccDistStartValue, int AroonOscPeriod,
                            int MACDPeriodShort, int MACDPeriodLong, int MACDPeriodSignal)
        {
            this.SMAPeriodShort = SMAPeriodShort;
            this.SMAPeriodLong = SMAPeriodLong;
            this.EMAPeriodShort = EMAPeriodShort;
            this.EMAPeriodLong = EMAPeriodLong;
            this.RSIPeriod = RSIPeriod;
            this.ROCPeriod = ROCPeriod;
            this.AccDistStartValue = AccDistStartValue;
            this.AroonOscPeriod = AroonOscPeriod;
            this.MACDPeriodLong = MACDPeriodLong;
            this.MACDPeriodShort = MACDPeriodShort;
            this.MACDPeriodSignal = MACDPeriodSignal;

            SMAShort = new Dictionary<string, decimal>();
            SMALong = new Dictionary<string, decimal>();
            EMAShort = new Dictionary<string, decimal>();
            EMALong = new Dictionary<string, decimal>();
            RSI = new Dictionary<string, decimal>();
            ROC = new Dictionary<string, decimal>();
            MACD = new Dictionary<string, decimal>();
            AccDist = new Dictionary<string, decimal>();
            AroonOsc = new Dictionary<string, decimal>();

        } 

        //void constructor (set the periods to their default values
        public AnalyticInfo() : this (15, 50, 12, 26, 14, 12, 0, 25, 12, 26, 9)
        {

        }

        #endregion

    }

    public class MarketData
    {
        public Stock stock;
        public DateTime date;
        public decimal marketType;
        public decimal minPrice, maxPrice;
        public decimal openPrice, closePrice;
        public decimal avgPrice;
        public decimal nOfNegotiations, nOfPapersTraded, volume;

    }

    //create an extension method to override the ToString method of the DateTimeClass
    //the format should be yyyymmdd
    public static class DateTimeExt
    {
        public static string ToStringF(this DateTime val)
        {
            string year, month, day;

            year = val.Year.ToString();

            if(val.Month < 10)
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
        public static int DaysDiff (this string s1, string s2)
        {
            DateTime d1 = new DateTime(int.Parse(s1.Substring(0, 4)), int.Parse(s1.Substring(4, 2)), int.Parse(s1.Substring(6, 2)));
            DateTime d2 = new DateTime(int.Parse(s2.Substring(0, 4)), int.Parse(s2.Substring(4, 2)), int.Parse(s2.Substring(6, 2)));

            return (int) (d1 - d2).TotalDays;
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
