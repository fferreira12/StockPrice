using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class StockComparer : IComparer<Stock>
    {
        #region fields

        private List<Stock> stocks;
        private List<Stock> rankedStocks;

        #endregion

        #region properties

        public List<Stock> Stocks
        {
            get
            {
                return stocks;
            }
            set
            {
                if(value != null)
                {
                    stocks = value;
                }
            }
        }

        public List<Stock> RankedStocks
        {
            get
            {
                return rankedStocks;
            }
            
        }

        #endregion

        #region constructors

        public StockComparer(List<Stock> stocks)
        {
            this.stocks = stocks;
        }

        public StockComparer(Dictionary<string, Stock> stocks)
        {

            List<Stock> lis =
                (from kvp in stocks
                 select kvp.Value).ToList();

            this.stocks = lis;
        }

        //void constructor to use IComparer
        public StockComparer()
        {

        }

        #endregion

        #region indexers

        public Stock this[int rank]
        {
            get
            {
                if(RankedStocks == null || rank > RankedStocks.Count)
                {
                    return null;
                }
                else
                {
                    return RankedStocks[rank - 1];
                }
            }
        }

        #endregion

        #region methods
        //testing
        public static List<Stock> RankOfBestStocks(List<Stock> allStks)
        {
            //verify if the indicators were not calculated
            //list of stocks that need to have indicators calculated
            //List<Stock> toCalc =
            //    (from st in allStks
            //     where st.indicators.Punctuation > 0
            //     select st).ToList();
            ////recalculate
            //MarketHistoryAnalyzer.FillAllWithDefaults(toCalc);
            ////foreach (Stock s in toCalc)
            ////{
            ////    MarketHistoryAnalyzer.FillAllWithDefaults(s);
            ////}

            ////get only stocks with indicators
            //var toCompare =
            //    from st in toCalc
            //    where st.indicators.QuantityOfIndicators > 0
            //    select st;

            //rank SMA
            //should be percentage
            var SMARank =
                (from Stock s in allStks
                 where s.indicators.SMAShort.Count()>0 && s.indicators.SMALong.Count() > 0 && s.MarketHistory.Count() > 0
                 orderby (s.indicators.SMAShort.Last().Value - s.indicators.SMALong.Last().Value) / s.MarketHistory.Last().closePrice descending
                 select s.stockCode).ToList();

            //rank EMA
            var EMARank =
                (from Stock s in allStks
                 where s.indicators.EMAShort.Count() > 0 && s.indicators.EMALong.Count() > 0 && s.MarketHistory.Count() > 0
                 orderby (s.indicators.EMAShort.Last().Value - s.indicators.EMALong.Last().Value) / s.MarketHistory.Last().closePrice descending
                 select s.stockCode).ToList();

            //rank RSI
            var RSIRank =
                (from Stock s in allStks
                 where s.indicators.RSI.Count > 0
                 orderby s.indicators.RSI.Last().Value descending
                 select s.stockCode).ToList();

            //rank Aroon
            var AroonRank =
                (from Stock s in allStks
                 where s.indicators.AroonOsc.Count > 0
                 orderby s.indicators.AroonOsc.Last().Value descending
                 select s.stockCode).ToList();

            //get values from the indexes of the ranks
            Dictionary<Stock, int> points = new Dictionary<Stock, int>();
            foreach (Stock stk in allStks)
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

        public static List<Stock> RankOfBestStocks(Dictionary<string, Stock> allStks)
        {
            var stks =
                (from s in allStks
                 select s.Value).ToList();

            return RankOfBestStocks(stks);
        } 

        public bool RankBestStocks(int quantity = 10)
        {
            //verify if the indicators were not calculated
            //list of stocks that need to have indicators calculated
            List<Stock> toCalc =
                (from st in stocks
                 //where st.indicators.Punctuation > 0
                 select st).ToList();
            //recalculate
            MarketHistoryAnalyzer.FillAllWithDefaults(toCalc);
            //foreach (Stock s in toCalc)
            //{
            //    MarketHistoryAnalyzer.FillAllWithDefaults(s);
            //}

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
                 select s.Key).Take(quantity).ToList();

            rankedStocks = rankList;

            return true;
        }

        public static List<Stock> RemoveStocksNotTradedEveryday(Dictionary<string, Stock> allStks)
        {
            //get number of days (max)
            int maxDays =
                (from s in allStks.Values
                 orderby s.MarketHistory.Count() descending
                 select s.MarketHistory.Count()).First();

            //remove ones with less than max
            List<Stock> allMax =
                (from s in allStks
                 where s.Value.MarketHistory.Count() == maxDays
                 select s.Value).ToList();

            return allMax;
        }

        public bool RankStocksByCompare()
        {
            if(stocks != null)
            {
                try
                {
                    rankedStocks = stocks.Clone().ToList();
                    rankedStocks.Sort(new StockComparer());
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public int Compare(Stock x, Stock y)
        {
            //starter tests
            if(x.MarketHistory == null || x.indicators == null || x.MarketHistory.Count() == 0 || x.indicators.QuantityOfIndicators == 0)
            {
                if ( y.MarketHistory == null || y.indicators == null || y.MarketHistory.Count() == 0 || y.indicators.QuantityOfIndicators == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }

            }

            //decimal xSum = 0, ySum = 0;

            //try
            //{
            //    xSum += x.indicators.SMAShort.Last().Value / x.indicators.SMALong.Last().Value;
            //}
            //catch (Exception)
            //{

            //}
            //try
            //{
            //    xSum += x.indicators.SMAShort.Last().Value / x.MarketHistory.Last().closePrice;
            //}
            //catch (Exception)
            //{

            //}
            //try
            //{
            //    xSum += x.indicators.EMAShort.Last().Value / x.indicators.EMALong.Last().Value;
            //}
            //catch (Exception)
            //{

            //}
            //try
            //{
            //    xSum += x.indicators.EMAShort.Last().Value / x.MarketHistory.Last().closePrice;
            //}
            //catch (Exception)
            //{

            //}
            //xSum += x.indicators.RSI.Last().Value / 100;
            //xSum += x.indicators.AroonOsc.Last().Value / 100;


            //try
            //{
            //    ySum += y.indicators.SMAShort.Last().Value / y.indicators.SMALong.Last().Value;
            //}
            //catch (Exception)
            //{

            //}
            //try
            //{
            //    ySum += y.indicators.SMAShort.Last().Value / y.MarketHistory.Last().closePrice;
            //}
            //catch (Exception)
            //{

            //}
            //try
            //{
            //    ySum += y.indicators.EMAShort.Last().Value / y.indicators.EMALong.Last().Value;
            //}
            //catch (Exception)
            //{

            //}
            //try
            //{
            //    ySum += y.indicators.EMAShort.Last().Value / y.MarketHistory.Last().closePrice;
            //}
            //catch (Exception)
            //{

            //}
            //ySum += y.indicators.RSI.Last().Value / 100;
            //ySum += y.indicators.AroonOsc.Last().Value / 100;

            return x.indicators.Punctuation.CompareTo(y.indicators.Punctuation) * -1;
        }

        #endregion

    }
}
