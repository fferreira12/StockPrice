using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class StockComparer
    {

        public static List<Stock> RankOfBestStocks(List<Stock> allStks)
        {
            //verify if the indicators were not calculated
            //list of stocks that need to have indicators calculated
            List<Stock> toCalc =
                (from st in allStks
                 where st.indicators.QuantityOfIndicators == 0 && st.MarketHistory.Count() > 0
                 select st).ToList();
            //recalculate
            MarketHistoryAnalyzer.FillAllWithDefaults(toCalc);

            //get only stocks with indicators
            var toCompare =
                from st in toCalc
                where st.indicators.QuantityOfIndicators > 0
                select st;

            //rank SMA
            //should be percentage
            var SMARank =
                (from Stock s in toCompare
                orderby (s.indicators.SMAShort.Last().Value - s.indicators.SMALong.Last().Value) / s.MarketHistory.Last().closePrice descending
                select s.stockCode).ToList();

            //rank EMA
            var EMARank =
                (from Stock s in toCompare
                orderby (s.indicators.EMAShort.Last().Value - s.indicators.EMALong.Last().Value) / s.MarketHistory.Last().closePrice descending
                select s.stockCode).ToList();

            //rank RSI
            var RSIRank =
                (from Stock s in toCompare
                orderby s.indicators.RSI descending
                select s.stockCode).ToList();

            //rank Aroon
            var AroonRank =
                (from Stock s in toCompare
                orderby s.indicators.AroonOsc descending
                select s.stockCode).ToList();

            //get values from the indexes of the ranks
            Dictionary<Stock, int> points = new Dictionary<Stock, int>();
            foreach (Stock stk in toCompare)
            {
                points.Add(stk, 0);
                points[stk] += SMARank.IndexOf(stk.stockCode);
                points[stk] += EMARank.IndexOf(stk.stockCode);
                points[stk] += RSIRank.IndexOf(stk.stockCode);
                points[stk] += AroonRank.IndexOf(stk.stockCode);
            }

            List<Stock> rankList =
                (from s in points
                 orderby s.Value
                 select s.Key).ToList();

            return rankList;


        }
        
        public static List<Stock> RankOfBestStocks(Dictionary<string,Stock> allStks)
        {
            var stks =
                (from s in allStks
                 select s.Value).ToList();

            return RankOfBestStocks(stks);
        }

    }
}
