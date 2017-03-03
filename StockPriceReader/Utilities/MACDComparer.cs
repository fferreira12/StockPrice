using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class MACDComparer : IComparer<Stock>
    {


        #region methods

        public static int GetDaysSinceLastReversal(Stock s)
        {
            if(s.indicators.MACD.Count == 0)
            {
                s.indicators.Recalculate();
            }


            string lastNegative =
                (from m in s.indicators.MACD
                 where m.Value < 0
                 select m.Key).LastOrDefault();
            string lastPositive =
                (from m in s.indicators.MACD
                 where m.Value > 0
                 select m.Key).LastOrDefault();

            decimal lastNegativeID = -1;
            decimal lastPositiveID = -1;

            if (lastNegative != null && lastPositive != null)
            {
                lastNegativeID = s.MarketHistory.Dates.IndexOf(lastNegative);
                lastPositiveID = s.MarketHistory.Dates.IndexOf(lastPositive); 
            }
            else
            {
                return -1;
            }

            decimal daysSinceReversal = Math.Abs(lastNegativeID - lastPositiveID);

            return (int)daysSinceReversal;
        }

        public static decimal GetMACDOpenness(Stock s)
        {
            if(s.indicators.MACD.Count == 0)
            {
                s.indicators.Recalculate();
            }
            return s.indicators.MACD.Last().Value / GetDaysSinceLastReversal(s);
        }

        public static List<Stock> GetRankOfMACDOpenness(Dictionary<string, Stock> allStocks)
        {
            SortedList<decimal, Stock> sList = new SortedList<decimal, Stock>();

            foreach (KeyValuePair<string,Stock> s in allStocks)
            {
                sList.Add(GetMACDOpenness(s.Value), s.Value);
            }

            sList.Reverse();

            List<Stock> lst = new List<Stock>(sList.Values);

            return lst;

        }

        public static List<Stock> GetRankOfMACDOpenness(List<Stock> allStocks)
        {
            return GetRankOfMACDOpenness(allStocks.ToDictionary((o) => o.stockCode, (o) => o));
        }

        #endregion

        #region interface methods

        public int Compare(Stock x, Stock y)
        {
            throw new NotImplementedException();
        } 

        #endregion
    }
}
