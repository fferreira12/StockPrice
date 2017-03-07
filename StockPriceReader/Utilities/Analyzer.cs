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
        //deprecated: use FillSimpleMovingAvg to add Analytic info to a stock
        public static decimal SimpleMovingAvg(Stock stk, int period, string referenceDate = "")
        {
            if (!IsCalculationPossible(stk, period))
                return 0m;

            //get all strings that represent dates (keys)
            List<string> dateKeys = stk.MarketDatas.Keys.ToList();

            //recent first (descending)
            List<string> orderedKeys = GetDescendingOrderedKeys(dateKeys, referenceDate);

            decimal sum = GetSumOfLastClosePrices(stk, period, orderedKeys);

            decimal avg = sum / period;

            return avg;
        }

        private static decimal GetSumOfLastClosePrices(Stock stk, int period, List<string> orderedKeys)
        {
            decimal sum = 0m;

            for (int i = 0; i < period; i++)
            {
                sum += stk.MarketDatas[orderedKeys[i]].closePrice;
            }

            return sum;
        }

        public static bool IsCalculationPossible(Stock stk, int period)
        {
            return !(stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < period);
        }

        public static List<string> GetDescendingOrderedKeys(List<string> allList, string referenceDate = "")
        {
            if (!referenceDate.Equals(""))
            {
                return 
                        (from key in allList
                         where key.CompareTo(referenceDate) <= 0 //get only items untill the specified date
                         orderby key descending
                         select key).ToList();
            }
            else
            {
                return 
                        (from key in allList
                         orderby key descending
                         select key).ToList();
            }
        }

        public static bool FillSimpleMovingAvg(ref Stock stk, int periodShort = 0, int periodLong = 0)
        {

            //check to see if the calculation is possible
            //if not, return false
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < periodShort ||
                (periodLong <= periodShort && periodLong != 0 && periodShort != 0))
            {
                return false;
            }

            //get default values for periods if zero
            if (periodShort == 0)
            {
                periodShort = stk.indicators.SMAPeriodShort;
            }
            if (periodLong == 0)
            {
                periodLong = stk.indicators.SMAPeriodLong;
            }

            //all dates
            List<string> allDates =
                (from d in stk.MarketDatas.Keys
                 orderby d
                 select d).ToList();

            //starter values (values at period-1)
            decimal sumShort = 0, sumLong = 0;
            for (int i = 0; i < periodLong; i++)
            {
                decimal actualVal = stk.MarketDatas[allDates[i]].closePrice;
                sumLong += actualVal;
                if (i < periodShort)
                {
                    sumShort += actualVal;
                }
            }

            //local collection to avoid constant getting to the object
            Dictionary<string, decimal> SMAShortValues = new Dictionary<string, decimal>();
            Dictionary<string, decimal> SMALongValues = new Dictionary<string, decimal>();

            for (int i = 0; i < allDates.Count; i++)
            {


                //testing purposes
                if (i == 2)
                {

                }


                //new value to add to both sums
                decimal newVal = 0;
                if (i-1 >= 0)
                {
                    newVal = stk.MarketDatas[allDates[i]].closePrice; 
                }

                //if short sma calc is possible
                if (i >= periodShort)
                {
                    sumShort -= stk.MarketDatas[allDates[i-periodShort]].closePrice;
                    sumShort += newVal;
                }

                //if long sma calc is possible
                if(i >= periodLong)
                {
                    sumLong -= stk.MarketDatas[allDates[i - periodLong]].closePrice;
                    sumLong += newVal;
                }

                
                //add zero if not yet possible
                if (i < periodLong-1)
                {
                    SMALongValues.Add(allDates[i], 0m);
                    if (i < periodShort-1)
                    {
                        SMAShortValues.Add(allDates[i], 0m);
                    }
                }

                decimal SMAShort = sumShort / periodShort;
                decimal SMALong = sumLong / periodLong;

                try
                {
                    SMAShortValues.Add(allDates[i], SMAShort);
                    SMALongValues.Add(allDates[i], SMALong);
                }
                catch (Exception)
                {
                    
                }
            }

            foreach (KeyValuePair<string, decimal> item in SMAShortValues)
            {
                try
                {
                    stk.indicators.SMAShort.Add(item.Key, item.Value);
                }
                catch (Exception)
                {

                }
            }
            foreach (KeyValuePair<string, decimal> item in SMALongValues)
            {
                try
                {
                    stk.indicators.SMALong.Add(item.Key, item.Value);
                }
                catch (Exception)
                {

                }
            }

            return true;
        }
        
        //deprecated
        public static decimal ExponentialMovingAvg(Stock stk, int period, string referenceDate = "")
        {
            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < period + 1)
            {
                return 0m;
            }


            List<string> allDates;

            if (referenceDate != "")
            {
                allDates =
                        (from string d in stk.MarketDatas.Keys
                         where d.CompareTo(referenceDate) <= 0 //get only items until the specified date
                         orderby d ascending
                         select d).ToList();
            }
            else
            {
                allDates =
                        (from string d in stk.MarketDatas.Keys
                         orderby d ascending
                         select d).ToList();
            }

            string startDate = string.Empty;
            try
            {
                startDate = allDates[period - 1];
            }
            catch (Exception)
            {

                return 0m;
            }

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

                decimal close = stk.MarketDatas[EMADates[i]].closePrice;
                todayEMA = ((close - todayEMA) * multiplier) + todayEMA;
                //lastDayEMA = todayEMA; //i could pretty much use just one variable that keeps changing itself
                
            }

            return todayEMA;
        }

        //deprecated
        public static decimal ExponentialMovingAvg(Dictionary<string, decimal> values, int period, string referenceDate = "") //, string referenceDate = "")
        {
            Stock stk = new Stock();
            foreach(KeyValuePair<string, decimal> kvp in values)
            {
                MarketData m = new MarketData();
                m.closePrice = kvp.Value;
                stk.MarketDatas.Add(kvp.Key, m);
            }
            decimal EMA = ExponentialMovingAvg(stk, period, referenceDate);
            return EMA;
        }

        public static bool FillExponentialMovingAvg(ref Stock stk, int periodShort = 0, int periodLong = 0)
        {
            //check to see if the calculation is possible
            //if not, return false
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < periodShort ||
                (periodLong <= periodShort && periodLong != 0 && periodShort != 0))
            {
                return false;
            }

            //get default values for periods if zero
            if (periodShort == 0)
            {
                periodShort = stk.indicators.EMAPeriodShort;
            }
            if (periodLong == 0)
            {
                periodLong = stk.indicators.EMAPeriodLong;
            }

            //all dates
            List<string> allDates =
                (from d in stk.MarketDatas.Keys
                 orderby d
                 select d).ToList();

            //starter values (values at period-1)
            //simple sum
            decimal avgShort = 0, avgLong = 0;
            for (int i = 0; i < periodLong; i++)
            {
                decimal actualVal = stk.MarketDatas[allDates[i]].closePrice;
                avgLong += actualVal;
                if (i < periodShort)
                {
                    avgShort += actualVal;
                }
            }
            avgShort /= periodShort;
            avgLong /= periodLong;

            //local collection to avoid constant getting to the object
            Dictionary<string, decimal> EMAShortValues = new Dictionary<string, decimal>();
            Dictionary<string, decimal> EMALongValues = new Dictionary<string, decimal>();

            decimal shortMultiplier = 2m / (periodShort + 1);
            decimal longMultiplier = 2m / (periodLong + 1);

            decimal newValShort = 0;
            decimal newValLong = 0;

            for (int i = 0; i < allDates.Count; i++)
            {

                //testing purposes
                if (i == 9)
                {

                }
                
                if (EMAShortValues.Count >= periodShort)
                {
                    newValShort = (stk.MarketDatas[allDates[i]].closePrice - newValShort) * shortMultiplier + newValShort;
                }
                else
                {
                    newValShort = avgShort;
                }

                if (EMALongValues.Count >= periodLong)
                {
                    newValLong = (stk.MarketDatas[allDates[i]].closePrice - newValLong) * longMultiplier + newValLong;
                }
                else
                {
                    newValLong = avgLong;
                }
                
                //add zero if not yet possible
                if (i < periodLong)
                {
                    EMALongValues.Add(allDates[i], 0m);
                    if (i < periodShort - 1)
                    {
                        EMAShortValues.Add(allDates[i], 0m);
                    }
                }
                
                try
                {
                    EMAShortValues.Add(allDates[i], newValShort);
                }
                catch (Exception)
                {

                }
                try
                {
                    EMALongValues.Add(allDates[i], newValLong);
                }
                catch (Exception)
                {

                }
            }

            foreach (KeyValuePair<string, decimal> item in EMAShortValues)
            {
                try
                {
                    stk.indicators.EMAShort.Add(item.Key, item.Value);
                }
                catch (Exception)
                {

                }
            }
            foreach (KeyValuePair<string, decimal> item in EMALongValues)
            {
                try
                {
                    stk.indicators.EMALong.Add(item.Key, item.Value);
                }
                catch (Exception)
                {

                }
            }

            return true;
        }

        //deprecated
        public static decimal AccumulationDistribution(Stock stk, string referenceDate = "", decimal startValue = 0m)
        {
            string date = string.Empty;

            //if reference date is empty, get he most recent one
            if (referenceDate == "")
            {

                date =
                    (from d in stk.MarketDatas.Keys
                     orderby d descending
                     select d).ToArray()[0];

            }
            else
            {
                date = referenceDate;
            }

            var orderedDates =
                from d in stk.MarketDatas.Keys
                where d.CompareTo(date) <= 0
                orderby d
                select d;

            //value at the start of 2016
            decimal AccDist = startValue; //- 620779409.2795m;
            decimal moneyFlow = 0m;

            foreach(string d in orderedDates)
            {
                MarketData mData = stk.MarketDatas[d];
                decimal close = mData.closePrice;
                decimal low = mData.minPrice;
                decimal high = mData.maxPrice;
                decimal volume = mData.nOfPapersTraded;

                moneyFlow = (((close - low) - (high - close)) / (high - low)) * volume;

                AccDist += moneyFlow;

            }

            return AccDist;
        }

        public static bool FillAccumulationDistribution(ref Stock stk, decimal startValue = 0m)
        {

            var orderedDates =
                from d in stk.MarketDatas.Keys
                orderby d
                select d;

            //value at the start of 2016
            decimal AccDist = startValue;
            decimal moneyFlow = 0m;

            Dictionary<string, decimal> allAccDists = new Dictionary<string, decimal>();

            foreach (string d in orderedDates)
            {
                MarketData mData = stk.MarketDatas[d];
                decimal close = mData.closePrice;
                decimal low = mData.minPrice;
                decimal high = mData.maxPrice;
                decimal volume = mData.nOfPapersTraded;

                moneyFlow = (((close - low) - (high - close)) / (high - low)) * volume;

                AccDist += moneyFlow;

                allAccDists.Add(d, AccDist);

            }

            foreach (KeyValuePair<string, decimal> item in allAccDists)
            {
                try
                {
                    stk.indicators.AccDist.Add(item.Key, item.Value);
                }
                catch(ArgumentException)
                {

                }
            }

            return true;
        }

        //deprecated
        public static decimal RelativeStrenghtIndex(Stock stk, int period, string referenceDate = "")
        {
            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < period + 1)
            {
                return 0m;
            }

            //all dates
            List<string> dates =
                (from string d in stk.MarketDatas.Keys
                orderby d
                select d).ToList();

            //changes
            List<decimal> changes = new List<decimal>();
            changes.Add(0m);
            for (int i = 1; i < dates.Count; i++) //start at the second (index 1)
            {
                decimal todayClose, yesterdayClose;
                todayClose = stk.MarketDatas[dates[i]].closePrice;
                yesterdayClose = stk.MarketDatas[dates[i-1]].closePrice;
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
                    (from d in stk.MarketDatas.Keys
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
            catch (DivideByZeroException)
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

        public static bool FillRelativeStrenghtIndex(Stock stk, int period = 0)
        {
            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < period + 1)
            {
                return false;
            }

            //get default period if not passed
            if(period == 0)
            {
                period = stk.indicators.RSIPeriod;
            }

            //all dates
            List<string> dates =
                (from string d in stk.MarketDatas.Keys
                 orderby d
                 select d).ToList();

            //changes
            List<decimal> changes = new List<decimal>();
            changes.Add(0m);
            decimal todayClose = 0, yesterdayClose;
            for (int i = 1; i < dates.Count; i++) //start at the second (index 1)
            {
                if (i == 1)
                {
                    yesterdayClose = stk.MarketDatas[dates[i - 1]].closePrice; 
                }
                else
                {
                    yesterdayClose = todayClose;
                }
                todayClose = stk.MarketDatas[dates[i]].closePrice;
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

            //get last date
            string date = dates.Last();
            
            //avg gain and loss
            decimal avgGain = startAvgGain;
            decimal avgLoss = startAvgLoss;

            //calculated RSIs
            Dictionary<string, decimal> allRSI = new Dictionary<string, decimal>();
            for (int i = 0; i<=period; i++)
            {
                if (i < period)
                {
                    allRSI.Add(dates[i], 0m); 
                }
                else
                {
                    allRSI.Add(dates[i], 100m - (100m / (1 + (avgGain / avgLoss))));
                }
            }
            for (int i = period + 1; i < dates.Count; i++)
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

                //relative strenght
                decimal rs;
                try
                {
                    rs = avgGain / avgLoss;
                }
                catch (DivideByZeroException)
                {
                    rs = 0m;
                }

                //relative strenght index
                decimal rsi;
                if (avgLoss != 0)
                {
                    rsi = 100m - (100m / (1 + rs));
                }
                else
                {
                    rsi = 100m;
                }

                allRSI.Add(dates[i], rsi);

            }

            foreach (KeyValuePair<string, decimal> item in allRSI)
            {
                try
                {
                    stk.indicators.RSI.Add(item.Key, item.Value);
                }
                catch (ArgumentException)
                {

                }
            }

            return true;

        }

        public static decimal AroonOscillator(Stock stk, int period, string referenceDate = "")
        {

            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < period + 1)
            {
                return 0m;
            }

            //allDates
            List<string> allDates =
                (from d in stk.MarketDatas.Keys
                 orderby d ascending
                 select d).ToList();


            Dictionary <string, MarketData> mData = new Dictionary<string, MarketData>();
            if(referenceDate == "")
            {

                referenceDate = allDates[allDates.Count-1];

            }

            for (int i = allDates.IndexOf(referenceDate) - period + 1; i <= allDates.IndexOf(referenceDate); i++)
            {
                try
                {
                    mData.Add(allDates[i], stk.MarketDatas[allDates[i]]);
                }
                catch (Exception)
                {
                    return 0m;
                }
            }

            //all close prices
            //for testing purposes
            List<decimal> allCloses = new List<decimal>();
            

            //days since period day high and low
            decimal highestVal = 0, lowestVal = 0; ;
            string highestDate = "", lowestDate = "";
            foreach(KeyValuePair<string, MarketData> kvp in mData)
            {
                //testing purposes
                allCloses.Add(kvp.Value.closePrice);
                if(highestVal == 0 && lowestVal == 0)
                {
                    highestVal = kvp.Value.closePrice;
                    highestDate = kvp.Key;
                    lowestVal = kvp.Value.closePrice;
                    lowestDate = kvp.Key;
                }
                else if(kvp.Value.closePrice >= highestVal)// && kvp.Key.CompareTo(highestDate) > 0)
                {
                    highestVal = kvp.Value.closePrice;
                    highestDate = kvp.Key;
                }
                else if(kvp.Value.closePrice <= lowestVal)// && kvp.Key.CompareTo(lowestDate) > 0)
                {
                    lowestVal = kvp.Value.closePrice;
                    lowestDate = kvp.Key;
                }

            }

            //days since
            int daysSinceLow = allDates.IndexOf(referenceDate) - allDates.IndexOf(lowestDate); //referenceDate.DaysDiff(lowestDate); 
            int daysSinceHigh =  allDates.IndexOf(referenceDate) - allDates.IndexOf(highestDate); //referenceDate.DaysDiff(highestDate); 

            //decimals
            decimal periodM = period;
            decimal daysSinceHighM = daysSinceHigh;
            decimal daysSinceLowM = daysSinceLow;

            //aroon up and down
            decimal aUp = 100m *  ((periodM - daysSinceHighM) / periodM);
            decimal aDown = 100m *  ((periodM - daysSinceLowM) / periodM);

            //arron oscillator
            decimal aOsc = aUp - aDown;

            return aOsc;

        }

        public static bool FillAroonOscillator(Stock stk, int period = 0)
        {
            //check to see if the calculation is possible
            //if not, return false
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < period + 1)
            {
                return false;
            }

            //get default period if zero
            if (period == 0)
            {
                period = stk.indicators.AroonOscPeriod;
            }

            //tried to make a local copy
            Dictionary<string, MarketData> localHist =
                (from x in stk.MarketDatas
                 select x).ToDictionary(x => x.Key, x => x.Value);

            //all dates
            List<string> allDates =
                (from d in localHist
                 orderby d.Key
                 select d.Key).ToList();

            Dictionary<string, decimal> allAOsc = new Dictionary<string, decimal>();

            for(int i = 0; i < allDates.Count; i++)
            {
                if (i < period)
                {
                    allAOsc.Add(allDates[i], 0m);
                }
                else
                {
                    //last n days values
                    var datesToConsider =
                        (from d in allDates
                         where allDates.IndexOf(d) >= i - period &&
                         allDates.IndexOf(d) < i
                         orderby d
                         select d);

                    //dates ordered ASCENDING by value
                    var dateRank =
                        (from d in datesToConsider
                         orderby localHist[d].closePrice ascending
                         select d);

                    //highest and lowest date
                    string highDate = dateRank.Last();
                    string lowDate = dateRank.First();

                    //days since high and low
                    int daysSinceHigh = i - allDates.IndexOf(highDate) - 1;
                    int daysSinceLow = i - allDates.IndexOf(lowDate) - 1;

                    //aroon up and down
                    decimal aUp = 100m * (period - daysSinceHigh) / period;
                    decimal aDown = 100m * (period - daysSinceLow) / period;

                    //aroon oscillator
                    decimal aOsc = aUp - aDown;

                    //add to local dictionary
                    allAOsc.Add(allDates[i], aOsc);
                }
            }

            foreach (KeyValuePair<string, decimal> item in allAOsc)
            {
                stk.indicators.AroonOsc.Add(item.Key, item.Value);
            }

            return true;

        }

        public static decimal MACD(Stock stk, int periodShort, int periodLong, int periodSignal, string referenceDate = "")
        {
            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < periodLong + 1)
            {
                return 0m;
            }

            //get most recent date if not passed
            if (referenceDate == "")
            {
                referenceDate =
                    (from date in stk.MarketDatas.Keys
                     orderby date descending
                     select date).ElementAt(0);
            }

            //get all dates
            var allDates =
                (from d in stk.MarketDatas.Keys
                 where d.CompareTo(referenceDate) <= 0
                 orderby d ascending
                 select d).Distinct().ToList();

            //all shortEmas
            Dictionary<string, decimal> allShortEmas = new Dictionary<string, decimal>();
            for(int i = periodShort+1; i <= allDates.IndexOf(referenceDate); i++)
            {
                decimal shortEma = ExponentialMovingAvg(stk, periodShort, allDates[i]);
                allShortEmas.Add(allDates[i], shortEma);
            }

            //all longEmas
            Dictionary<string, decimal> allLongEmas = new Dictionary<string, decimal>();
            for (int i = periodLong + 1; i <= allDates.IndexOf(referenceDate); i++)
            {
                decimal longEma = ExponentialMovingAvg(stk, periodLong, allDates[i]);
                allLongEmas.Add(allDates[i], longEma);
            }

            //all MACDs
            Dictionary<string, decimal> allMACDs = new Dictionary<string, decimal>();
            foreach(KeyValuePair<string, decimal> kvp in allLongEmas)
            {
                decimal MACD = allShortEmas[kvp.Key] - kvp.Value;
                allMACDs.Add(kvp.Key, MACD);
            }

            //signal line
            //PROBLEMATIC
            Dictionary<string, decimal> signalLine = new Dictionary<string, decimal>();

            foreach (var date in allDates)
            {
                decimal signal = 0m;
                signal = ExponentialMovingAvg(allMACDs, periodSignal, date);
                signalLine.Add(date, signal);
            }

            //MACD histogram
            int startPeriod = periodLong > periodSignal ? periodLong + 1 : periodSignal + 1;
            Dictionary<string, decimal> MACDHist = new Dictionary<string, decimal>();
            for (int i = startPeriod; i <= allDates.IndexOf(referenceDate); i++)
            {
                decimal MACDHistogram = allMACDs[allDates[i]] - signalLine[allDates[i]];
                MACDHist.Add(allDates[i], MACDHistogram);
            }

            //for testing purposes
            string recentDate = referenceDate;
            decimal actualMACD = allMACDs[recentDate];
            decimal actualSignal = signalLine[recentDate];
            decimal actualHist = MACDHist[recentDate];

            return MACDHist[referenceDate];

        }

        public static decimal RateOfChange(Stock stk, int period, string referenceDate = "")
        {

            //check to see if the calculation is possible
            //if not, return zero
            if (stk == null || stk.MarketDatas == null || stk.MarketDatas.Count == 0 || stk.MarketDatas.Count < period + 1)
            {
                return 0m;
            }

            //allDates
            List<string> allDates =
                (from d in stk.MarketDatas.Keys
                 orderby d
                 select d).ToList();

            ////all ROCs
            ////not all for performance issues
            //Dictionary<string, decimal> allROCs = new Dictionary<string, decimal>();
            //for (int i = allDates.Count - period; i < allDates.Count; i++)
            //{
            //    //close price of the day
            //    decimal actualClose = stk.marketHistory[allDates[i]].closePrice;

            //    //try to get the price n days before
            //    //if not possible, return zero
            //    decimal oldClose = 0m;
            //    try
            //    {
            //        oldClose = stk.marketHistory[allDates[i-period]].closePrice;
            //    }
            //    catch (Exception)
            //    {
            //        allROCs.Add(allDates[i], 0m);
            //        continue;
            //    }

            //    decimal ROC = 100 * (actualClose - oldClose) / oldClose;

            //    allROCs.Add(allDates[i], ROC);
            //}
            decimal actualClose = 0m;
            decimal oldClose = 0m;
            if (referenceDate == "")
            {
                actualClose = stk.MarketDatas[allDates.Last()].closePrice;
                oldClose = stk.MarketDatas[allDates[allDates.Count - period]].closePrice;
                return 100 * (actualClose - oldClose) / oldClose;
            }
            else
            {
                actualClose = stk.MarketDatas[referenceDate].closePrice;
                oldClose = stk.MarketDatas[allDates[allDates.IndexOf(referenceDate) - period]].closePrice;
                return 100 * (actualClose - oldClose) / oldClose;
            }
        }

        //extremely slow, does not work
        public static bool AnalyzeStock(Stock stk)
        {
            try
            {
                AnalyticInfo indics = stk.indicators;

                List<string> allDates = stk.MarketDatas.Keys.OrderBy(o => o).ToList();

                for (int i = 0; i < allDates.Count; i++)
                {
                    try
                    {

                        decimal SMAShort, SMALong, EMAShort, EMALong, accDist, RSI, AroonOsc, Macd, Roc;
                        //produce indicators for that date
                        try
                        {
                            SMAShort = SimpleMovingAvg(stk, indics.SMAPeriodShort, allDates[i]);
                        }
                        catch (Exception)
                        {

                            SMAShort = 0;
                        }
                        try
                        {
                            SMALong = SimpleMovingAvg(stk, indics.SMAPeriodLong, allDates[i]);
                        }
                        catch (Exception)
                        {

                            SMALong = 0;
                        }
                        try
                        {
                            EMAShort = ExponentialMovingAvg(stk, indics.EMAPeriodShort, allDates[i]);
                        }
                        catch (Exception)
                        {

                            EMAShort = 0;
                        }
                        try
                        {
                            EMALong = ExponentialMovingAvg(stk, indics.EMAPeriodLong, allDates[i]);
                        }
                        catch (Exception)
                        {

                            EMALong = 0;
                        }
                        try
                        {
                            accDist = AccumulationDistribution(stk, allDates[i]);
                        }
                        catch (Exception)
                        {

                            accDist = 0;
                        }
                        try
                        {
                            RSI = RelativeStrenghtIndex(stk, indics.RSIPeriod, allDates[i]);
                        }
                        catch (Exception)
                        {

                            RSI = 0;
                        }
                        try
                        {
                            AroonOsc = AroonOscillator(stk, indics.AroonOscPeriod, allDates[i]);
                        }
                        catch (Exception)
                        {

                            AroonOsc = 0;
                        }
                        try
                        {
                            Macd = MACD(stk, indics.MACDPeriodShort, indics.MACDPeriodLong, indics.MACDPeriodSignal, allDates[i]);
                        }
                        catch (Exception)
                        {

                            Macd = 0;
                        }
                        try
                        {
                            Roc = RateOfChange(stk, indics.ROCPeriod, allDates[i]);
                        }
                        catch (Exception)
                        {

                            Roc = 0;
                        }

                        //add indicators to the Analytic info of the stock object
                        indics.SMAShort.Add(allDates[i], SMAShort);
                        indics.SMALong.Add(allDates[i], SMALong);
                        indics.EMAShort.Add(allDates[i], EMAShort);
                        indics.EMALong.Add(allDates[i], EMALong);
                        indics.AccDist.Add(allDates[i], accDist);
                        indics.RSI.Add(allDates[i], RSI);
                        indics.AroonOsc.Add(allDates[i], AroonOsc);
                        indics.MACD.Add(allDates[i], Macd);
                        indics.ROC.Add(allDates[i], Roc);
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
