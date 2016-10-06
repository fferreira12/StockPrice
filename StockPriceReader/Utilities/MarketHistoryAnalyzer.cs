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

            foreach (MarketData m in stk.MarketHistory)
            {
                var lastClosesShort = stk.MarketHistory.GetLastNClosingPrices(periodShort, m.dateStr);
                var lastMClosesLong = stk.MarketHistory.GetLastNClosingPrices(periodLong, m.dateStr);

                decimal avgShort = lastClosesShort.Average();
                decimal avgLong = lastMClosesLong.Average();

                stk.indicators.EMALong.Add(m.dateStr, avgLong);
                stk.indicators.EMAShort.Add(m.dateStr, avgShort);

                valuesAdded += 2;
            }

            return valuesAdded > 0;
        }

    }
}
