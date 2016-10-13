using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    class Comparer
    {

        public static List<Stock> RankOfBestStocks(List<Stock> allStks)
        {
            //rank SMA
            //should be percentage
            var SMARank =
                from Stock s in allStks
                orderby (s.indicators.SMAShort.Last().Value - s.indicators.SMALong.Last().Value) / s.MarketHistory.Last().closePrice descending
                select s;

            //rank EMA
            var EMARank =
                from Stock s in allStks
                orderby (s.indicators.EMAShort.Last().Value - s.indicators.EMALong.Last().Value) / s.MarketHistory.Last().closePrice descending
                select s;

            //rank RSI
            var RSIRank =
                from Stock s in allStks
                orderby s.indicators.RSI descending
                select s;

            //rank Aroon
            var AroonRank =
                from Stock s in allStks
                orderby s.indicators.AroonOsc descending
                select s;

        }
        
    }
}
