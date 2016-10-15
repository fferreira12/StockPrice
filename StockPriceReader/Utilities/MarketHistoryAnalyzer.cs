using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

    /*
    This class role is to fill up the indicator in a stock
    It will replace the Analyzer Class
    */

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

                //short eval
                if (lastClosesShort.Count() == periodShort)
                {
                    try
                    {
                        stk.indicators.SMAShort.Add(m.dateStr, avgShort);
                    }
                    catch (Exception)
                    {
                        stk.indicators.SMAShort[m.dateStr] = avgShort;
                    }
                }
                else
                {
                    try
                    {
                        stk.indicators.SMAShort.Add(m.dateStr, 0m);
                    }
                    catch
                    {
                        stk.indicators.SMAShort[m.dateStr] = 0m;
                    }
                }

                //long eval
                if (lastMClosesLong.Count() == periodLong)
                {
                    try
                    {
                        stk.indicators.SMALong.Add(m.dateStr, avgLong);
                    }
                    catch (Exception)
                    {
                        stk.indicators.SMALong[m.dateStr] = avgLong;
                    }
                }
                else
                {
                    try
                    {
                        stk.indicators.SMALong.Add(m.dateStr, 0m);
                    }
                    catch
                    {
                        stk.indicators.SMALong[m.dateStr] = 0m;

                    }
                }

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

        public static bool FillAccDist(ref Stock stk, decimal startValue = 0m)
        {

            //preliminar tests
            if (stk == null || stk.MarketHistory == null || stk.MarketHistory.Dates.Count == 0)
            {
                return false;
            }

            //local temporary Dictionary<string
            Dictionary<string, decimal> allAccDist = new Dictionary<string, decimal>();
            decimal actualAccDist = startValue;
            foreach (MarketData m in stk.MarketHistory)
            {
                int index = stk.MarketHistory.GetIndexOfDate(m);
                
                if(index == 0)
                {
                    allAccDist.Add(m.dateStr, actualAccDist);
                }
                else
                {

                    decimal moneyFlowMultiplier = 0;
                    try
                    {
                        moneyFlowMultiplier = ((m.closePrice - m.minPrice) - (m.maxPrice - m.closePrice)) / (m.maxPrice - m.minPrice);
                    }
                    catch (DivideByZeroException)
                    {
                        moneyFlowMultiplier = 1m; //one or zero?
                    }
                    decimal moneyFLowVolume = moneyFlowMultiplier * m.nOfPapersTraded;
                    actualAccDist += moneyFLowVolume;
                    allAccDist.Add(m.dateStr, actualAccDist);

                }
            }
            if(allAccDist.Count == 0)
            {
                return false;
            }
            foreach(KeyValuePair<string, decimal> item in allAccDist)
            {
                try
                {
                    stk.indicators.AccDist.Add(item.Key, item.Value);
                }
                catch (Exception)
                {
                    stk.indicators.AccDist[item.Key] = item.Value;

                }
            }
            return true;

        }

        public static bool FillRSI(ref Stock stk, int period = 0)
        {
            //preliminar tests
            if (stk == null || stk.MarketHistory == null || stk.MarketHistory.Dates.Count == 0)
            {
                return false;
            }

            //get default period if zero
            if(period == 0)
            {
                period = stk.indicators.RSIPeriod;
            }

            //it probably does NOT clone the object
            Stock localStk = stk;

            //first average gain and loss
            var gains =
                (from d in localStk.MarketHistory.Dates
                 where localStk.MarketHistory.PreviousMarketData(d) != null &&
                 localStk.MarketHistory[d].closePrice - localStk.MarketHistory.PreviousMarketData(d).closePrice >= 0m &&
                 localStk.MarketHistory.GetIndexOfDate(d) < period
                 orderby d
                 select localStk.MarketHistory[d].closePrice - localStk.MarketHistory.PreviousMarketData(d).closePrice);
            decimal avgGain = gains.Sum() / period;
            var losses =
                (from d in localStk.MarketHistory.Dates
                 where localStk.MarketHistory.PreviousMarketData(d) != null &&
                 localStk.MarketHistory[d].closePrice - localStk.MarketHistory.PreviousMarketData(d).closePrice <= 0m &&
                 localStk.MarketHistory.GetIndexOfDate(d) < period
                 orderby d
                 select Math.Abs(localStk.MarketHistory[d].closePrice - localStk.MarketHistory.PreviousMarketData(d).closePrice));
            decimal avgLoss = losses.Sum() / period;

            //local dic for temporary storing values
            Dictionary<string, decimal> allRSI = new Dictionary<string, decimal>();

            //other gains and losses
            foreach (MarketData m in stk.MarketHistory)
            {
                int index = stk.MarketHistory.GetIndexOfDate(m);
                if(index > period)
                {
                    //test purposes
                    if(avgGain < 0 || avgLoss < 0)
                    {
                        throw new Exception("Médias de ganhos e perdas não podem ser negativas");
                    }
                    
                    decimal change = m.closePrice - stk.MarketHistory.GetLastNClosingPrices(2, m.dateStr).First();
                    avgGain = change >= 0 ? (avgGain * (period-1) + change) / period : (avgGain * (period-1) + 0m) / period;
                    avgLoss = change <= 0 ? (avgLoss * (period-1) + Math.Abs(change)) / period : (avgLoss * (period-1) + 0m) / period;
                    decimal RS = avgGain / avgLoss;
                    decimal RSI = avgLoss == 0m ? 100m : 100m - (100m / (1m + RS));
                    allRSI.Add(m.dateStr, RSI); 
                }
                else if (index == period)
                {
                    decimal RS = avgGain / avgLoss;
                    decimal RSI = avgLoss == 0m ? 100m : 100m-(100m/(1m+RS));
                    allRSI.Add(m.dateStr, RSI);
                }
                else
                {
                    allRSI.Add(m.dateStr, 0m);
                }
            }

            if (allRSI.Count == 0)
            {
                return false; 
            }

            foreach(KeyValuePair<string, decimal> item in allRSI)
            {
                try
                {
                    stk.indicators.RSI.Add(item.Key, item.Value);
                }
                catch (Exception)
                {
                    stk.indicators.RSI[item.Key] = item.Value;
                }
            }

            return true;

        }

        public static bool FillAroon(ref Stock stk, int period = 0)
        {
            //preliminar tests
            if (stk == null || stk.MarketHistory == null || stk.MarketHistory.Dates.Count == 0)
            {
                return false;
            }

            //get defaults if zero
            if (period == 0)
            {
                period = stk.indicators.AroonOscPeriod;
            }

            //local collections
            Dictionary<string, decimal> aUps = new Dictionary<string, decimal>();
            Dictionary<string, decimal> aDowns = new Dictionary<string, decimal>();
            Dictionary<string, decimal> aOscs = new Dictionary<string, decimal>();

            //general iterator
            foreach (MarketData m in stk.MarketHistory)
            {
                int index = stk.MarketHistory.GetIndexOfDate(m);
                if(index < period)
                {
                    aUps.Add(m.dateStr, 0m);
                    aDowns.Add(m.dateStr, 0m);
                    aOscs.Add(m.dateStr, 0m);
                }
                else
                {
                    //last n days closing prices
                    IEnumerable<MarketData> lastMDatas = stk.MarketHistory.GetLastNMarKetDatas(period, m.dateStr);

                    //order by closing price
                    IEnumerable<MarketData> orderedMDatas =
                        from md in lastMDatas
                        orderby md.closePrice ascending
                        select md;

                    //high and low indexes
                    MarketData low = orderedMDatas.First();
                    MarketData high = orderedMDatas.Last();

                    //days since high and low
                    decimal daysSinceHigh = stk.MarketHistory.Dates.IndexOf(m.dateStr) - stk.MarketHistory.Dates.IndexOf(high.dateStr);
                    decimal daysSinceLow = stk.MarketHistory.Dates.IndexOf(m.dateStr) - stk.MarketHistory.Dates.IndexOf(low.dateStr);

                    //the max value daysSince can achieve is period-1
                    //this way the range of aUp and aDown will vary from 0 to 100
                    decimal aUp = 100m * ((period-1) - daysSinceHigh) / (period-1);
                    decimal aDown = 100m * ((period-1) - daysSinceLow) / (period-1);
                    decimal aOsc = aUp - aDown;

                    //testing purposes
                    //if (m.dateStr == "20160926")
                    //{

                    //}

                    aUps.Add(m.dateStr, aUp);
                    aDowns.Add(m.dateStr, aDown);
                    aOscs.Add(m.dateStr, aOsc);
                }
            }

            List<string> dates = stk.MarketHistory.Dates;

            for (int i = 0; i < aOscs.Count(); i++)
            {
                try
                {
                    stk.indicators.AroonOsc.Add(dates[i], aOscs[dates[i]]);
                }
                catch (Exception)
                {

                }
                try
                {
                    stk.indicators.AroonUp.Add(dates[i], aUps[dates[i]]);
                }
                catch (Exception)
                {

                }
                try
                {
                    stk.indicators.AroonDown.Add(dates[i], aDowns[dates[i]]);
                }
                catch (Exception)
                {

                }
            }

            return true;

        }

        public static bool FillMACD(ref Stock stk, int periodShort = 0, int periodLong = 0, int periodSignal = 0)
        {
            //preliminar tests
            if (stk == null || stk.MarketHistory == null || stk.MarketHistory.Dates.Count == 0)
            {
                return false;
            }

            //get defaults if zero
            periodShort = periodShort == 0 ? stk.indicators.MACDPeriodShort : periodShort;
            periodLong = periodLong == 0 ? stk.indicators.MACDPeriodLong : periodLong;
            periodSignal = periodSignal == 0 ? stk.indicators.MACDPeriodSignal : periodSignal;

            //fill up the moving avgs that will be used
            FillExponentialMovingAvg(ref stk, periodShort, periodLong);

            List<string> allDates = stk.MarketHistory.Dates;
            for (int i = 0; i < stk.indicators.EMALong.Count; i++)
            {
                if(i < stk.indicators.EMAPeriodLong)
                {
                    try
                    {
                        stk.indicators.MACD.Add(allDates[i], 0m);
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    try
                    {
                        stk.indicators.MACD.Add(allDates[i], stk.indicators.EMAShort[allDates[i]] - stk.indicators.EMALong[allDates[i]]);
                    }
                    catch (Exception)
                    {

                    }

                }
            }

            return true;
        }

        public static bool FillROC(ref Stock stk, int period = 0)
        {
            //preliminar tests
            if (stk == null || stk.MarketHistory == null || stk.MarketHistory.Dates.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < stk.MarketHistory.Count(); i++)
            {
                //all dates
                List<string> dates = stk.MarketHistory.Dates;

                //get defaults if zero
                period = period == 0 ? stk.indicators.ROCPeriod : period;

                if (i < period)
                {
                    try
                    {
                        stk.indicators.ROC.Add(dates[i], 0m);
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    decimal nowClose = stk.MarketHistory[i].closePrice;
                    decimal oldClose = stk.MarketHistory[i - period].closePrice;
                    try
                    {
                        stk.indicators.ROC.Add(dates[i], 100m * (nowClose - oldClose) / oldClose);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return true;
        }

        public static bool FillAllWithDefaults(Stock stk)
        {
            //preliminar tests
            if (stk == null || stk.MarketHistory == null || stk.MarketHistory.Dates.Count == 0)
            {
                return false;
            }

            FillSimpleMovingAvg(ref stk);
            //FillExponentialMovingAvg(ref stk); //the FillMACD already fills the EMAs
            FillAccDist(ref stk);
            FillRSI(ref stk);
            FillAroon(ref stk);
            FillMACD(ref stk);
            FillROC(ref stk);

            return true;
        }

        public static bool FillAllWithDefaults(List<Stock> allStks)
        {
            //attempt to make it parallel
            ConcurrentQueue<Stock> concStocks = new ConcurrentQueue<Stock>();

            //preliminar tests
            if(allStks == null || allStks.Count == 0)
            {
                return false;
            }

            for(int i = 0; i < allStks.Count(); i++)
            {
                //FillAllWithDefaults(allStks[i]);
                concStocks.Enqueue(allStks[i]);
            }

            Action fillUp = ( () =>
            {
                Stock s = null;

                if(concStocks.TryDequeue(out s))
                {
                    FillAllWithDefaults(s);
                }

            });

            Action[] actArr = new Action[allStks.Count];

            for(int i = 0; i < allStks.Count; i++)
            {
                actArr[i] = fillUp;
            }

            Parallel.Invoke(fillUp, fillUp, fillUp, fillUp, fillUp);

            return true;
        }

        public static bool FillAllWithDefaults(Dictionary<string, Stock> allStks)
        {
            //preliminar tests
            if (allStks == null || allStks.Count == 0)
            {
                return false;
            }

            List<Stock> stksList = new List<Stock>();

            foreach (KeyValuePair<string, Stock> kvp in allStks)
            {
                stksList.Add(kvp.Value);
            }

            return FillAllWithDefaults(stksList);

        }
    }
}
