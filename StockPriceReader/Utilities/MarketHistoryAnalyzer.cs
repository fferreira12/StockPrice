using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class MarketHistoryAnalyzer
    {

        public static bool FillSimpleMovingAvg(ref Stock stk, int periodShort = 0, int periodLong = 0)
        {
            //get defaults if zero
            if(periodShort == 0)
            {
                periodShort = stk.indicators.SMAPeriodShort;
            }
            if (periodLong == 0)
            {
                periodLong = stk.indicators.SMAPeriodLong;
            }

            int valuesAdded = 0;

            Dictionary<string, decimal> tempLong = new Dictionary<string, decimal>();
            Dictionary<string, decimal> tempShort = new Dictionary<string, decimal>();

            foreach (MarketData m in stk.MarketHistory)
            {
                var lastClosesShort = stk.MarketHistory.GetLastNClosingPrices(periodShort, m.dateStr);
                var lastMClosesLong = stk.MarketHistory.GetLastNClosingPrices(periodLong, m.dateStr);

                decimal avgShort = lastClosesShort.Average();
                decimal avgLong = lastMClosesLong.Average();

                stk.indicators.SMALong.Add(m.dateStr, avgLong);
                stk.indicators.SMAShort.Add(m.dateStr, avgShort);

                tempLong.Add(m.dateStr, avgLong);
                tempShort.Add(m.dateStr, avgShort);

                valuesAdded += 2;
            }

            return valuesAdded > 0;
        }

        public static bool FillExponentialMovingAvg(ref Stock stk, int periodShort = 0, int periodLong = 0)
        {
            //preliminar tests
            if(stk == null || stk.MarketHistory == null || stk.MarketHistory.Dates.Count == 0)
            {
                return false;
            }

            //get defaults if zero
            if (periodShort == 0)
            {
                periodShort = stk.indicators.EMAPeriodShort;
            }
            if (periodLong == 0)
            {
                periodLong = stk.indicators.EMAPeriodLong;
            }

            //starter values
            decimal starterShort = stk.MarketHistory.GetLastNClosingPrices(periodShort, stk.MarketHistory[periodShort].dateStr).Average();
            decimal starterLong = stk.MarketHistory.GetLastNClosingPrices(periodLong, stk.MarketHistory[periodLong].dateStr).Average();

            //multipliers
            decimal multShort = (2m / (periodShort + 1m));
            decimal multLong = (2m / (periodLong + 1m));

            //temporary local results
            Dictionary<string, decimal> tempShort = new Dictionary<string, decimal>();
            Dictionary<string, decimal> tempLong = new Dictionary<string, decimal>();

            //auxiliars
            decimal lastShort = 0, todayShort = 0, lastLong = 0, todayLong = 0;

            foreach (MarketData m in stk.MarketHistory)
            {
                //get current index
                int actualIndex = stk.MarketHistory.GetIndexOfDate(m);

                //add zeroes before the correct number of periods
                if(actualIndex < periodLong)
                {
                    tempLong.Add(m.dateStr, 0m);
                    if(actualIndex < periodShort)
                    {
                        tempShort.Add(m.dateStr, 0m);
                    }
                }

                if(actualIndex == periodShort)
                {
                    tempShort.Add(m.dateStr, starterShort);
                    //if period long = period short
                    if (actualIndex == periodLong)
                    {
                        tempLong.Add(m.dateStr, starterLong);
                    }
                }

                if (actualIndex == periodLong)
                {
                    tempLong.Add(m.dateStr, starterLong);
                }

                if(actualIndex > periodShort)
                {
                    lastShort = todayShort != 0m ? todayShort : tempShort.Last().Value;
                    todayShort = ((m.closePrice - lastShort) * multShort) + lastShort;
                    tempShort.Add(m.dateStr, todayShort);

                    if (actualIndex > periodLong)
                    {
                        lastLong = todayLong != 0m ? todayLong : tempLong.Last().Value;
                        todayLong = ((m.closePrice - lastLong) * multLong) + lastLong;
                        try
                        {
                            tempLong.Add(m.dateStr, todayLong);
                        }
                        catch (ArgumentException)
                        {
                            //if item was already added do nothing
                        }
                    }

                }

            }

            foreach(KeyValuePair<string, decimal> item in tempLong)
            {
                stk.indicators.EMALong.Add(item.Key, item.Value);
            }

            foreach (KeyValuePair<string, decimal> item in tempShort)
            {
                stk.indicators.EMAShort.Add(item.Key, item.Value);
            }

            return true;
        }

    }
}
