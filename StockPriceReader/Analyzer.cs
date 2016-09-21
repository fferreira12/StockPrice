using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

    /*
        
        This class' role is to create information about stocks
        based on its market data
        
        Mostly through the use of indexes and indicators   
         
    */

    public class Analyzer
    {

        //uses closing prices
        public static decimal SimpleMovingAvg(Stock stk, int period, string referenceDate = "")
        {
            //check to see if the calculation is possible
            //if not, return zero
            if(stk == null || stk.marketHistory == null || stk.marketHistory.Count == 0 || stk.marketHistory.Count < period)
            {
                return 0m;
            }

            //get all strings that represent dates (keys)
            List<string> dateKeys = stk.marketHistory.Keys.ToList();
            
            //recent first (descending)
            List<string> orderedKeys;

            if (!referenceDate.Equals(""))
            {
                orderedKeys =
                        (from key in dateKeys
                         where key.CompareTo(referenceDate) <= 0 //get only items untill the specified date
                         orderby key descending
                         select key).ToList(); 
            }
            else
            {
                orderedKeys =
                        (from key in dateKeys
                         orderby key descending
                         select key).ToList();
            }

            decimal sum = 0m;

            for(int i = 0; i < period; i++)
            {
                sum += stk.marketHistory[orderedKeys[i]].closePrice;
            }

            decimal avg = sum / period;

            return avg;
        }

        public static decimal ExponentialMovingAvg(Stock stk, int period, string referenceDate = "")
        {
            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.marketHistory == null || stk.marketHistory.Count == 0 || stk.marketHistory.Count < period + 1)
            {
                return 0m;
            }


            List<string> allDates;

            if (referenceDate != "")
            {
                allDates =
                        (from string d in stk.marketHistory.Keys
                         where d.CompareTo(referenceDate) <= 0 //get only items untill the specified date
                         orderby d ascending
                         select d).ToList();
            }
            else
            {
                allDates =
                        (from string d in stk.marketHistory.Keys
                         orderby d ascending
                         select d).ToList();
            }

            string startDate = allDates[period-1];

            //simple moving average as starting point
            decimal SMA = SimpleMovingAvg(stk, period, startDate);

            List<string> EMADates =
                (from d in allDates
                 where d.CompareTo(startDate) > 0
                 select d).ToList();

            decimal multiplier = 2m / (period + 1m);

            //decimal lastDayEMA = 0;
            decimal todayEMA = SMA;

            for(int i = 0; i < EMADates.Count; i++)
            {

                decimal close = stk.marketHistory[EMADates[i]].closePrice;
                todayEMA = ((close - todayEMA) * multiplier) + todayEMA;
                //lastDayEMA = todayEMA; //i could pretty much use just one variable that keeps changing itself
                
            }

            return todayEMA;
        }

        public static decimal AccumulationDistribution(Stock stk, string referenceDate = "", decimal startValue = 0m)
        {
            string date = string.Empty;

            //if reference date is empty, get he most recent one
            if (referenceDate == "")
            {

                date =
                    (from d in stk.marketHistory.Keys
                     orderby d descending
                     select d).ToArray()[0];

            }
            else
            {
                date = referenceDate;
            }

            var orderedDates =
                from d in stk.marketHistory.Keys
                where d.CompareTo(date) <= 0
                orderby d
                select d;

            //value at the start of 2016
            decimal AccDist = startValue; //- 620779409.2795m;
            decimal moneyFlow = 0m;

            foreach(string d in orderedDates)
            {
                MarketData mData = stk.marketHistory[d];
                decimal close = mData.closePrice;
                decimal low = mData.minPrice;
                decimal high = mData.maxPrice;
                decimal volume = mData.nOfNegotiations;

                moneyFlow = (((close - low) - (high - close)) / (high - low)) * volume;

                AccDist += moneyFlow;

            }

            return AccDist;
        }

        public static decimal RelativeStrenghtIndex(Stock stk, int period, string referenceDate = "")
        {
            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.marketHistory == null || stk.marketHistory.Count == 0 || stk.marketHistory.Count < period + 1)
            {
                return 0m;
            }

            //all dates
            List<string> dates =
                (from string d in stk.marketHistory.Keys
                orderby d
                select d).ToList();

            //changes
            List<decimal> changes = new List<decimal>();
            changes.Add(0m);
            for (int i = 1; i < dates.Count; i++) //start at the second (index 1)
            {
                decimal todayClose, yesterdayClose;
                todayClose = stk.marketHistory[dates[i]].closePrice;
                yesterdayClose = stk.marketHistory[dates[i-1]].closePrice;
                changes.Add(todayClose - yesterdayClose);
            }

            //start avg gain and loss
            decimal startAvgGain = 0m;
            decimal startAvgLoss = 0m;
            for (int i = 1; i < period; i++)
            {
                if (changes[i] > 0)
                {
                    startAvgGain += Math.Abs(changes[i]); 
                }
                else
                {
                    startAvgLoss += Math.Abs(changes[i]);
                }
            }
            startAvgGain /= period;
            startAvgLoss /= period;

            //get date
            string date = string.Empty;
            if (referenceDate == "")
            {
                date =
                    (from d in stk.marketHistory.Keys
                     orderby d descending
                     select d).ToArray()[0];

            }
            else
            {
                date = referenceDate;
            }

            //avg gain and loss
            decimal avgGain = startAvgGain;
            decimal avgLoss = startAvgLoss;

            for (int i = period+1; i < dates.IndexOf(date); i++)
            {
                decimal gain, loss;
                if (changes[i] > 0)
                {
                    gain = Math.Abs(changes[i]);
                    loss = 0m;
                }
                else if (changes[i] < 0)
                {
                    gain = 0m;
                    loss = Math.Abs(changes[i]);
                }
                else
                {
                    gain = 0m;
                    loss = 0m;
                }
                avgGain = (avgGain * (period - 1) + gain) / period;
                avgLoss = (avgLoss * (period - 1) + loss) / period;
            }

            //relative strenght
            decimal rs;

            try
            {
                rs = avgGain / avgLoss;
            }
            catch (DivideByZeroException ex)
            {
                return 0m;
            }

            //relative strenght index
            decimal rsi;
            
            if(avgLoss != 0)
            {
                rsi = 100m - (100m / (1 + rs));
            }
            else
            {
                rsi = 100m;
            }

            return rsi;
        }
    }
}
