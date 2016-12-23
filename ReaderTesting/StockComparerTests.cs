using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;

namespace ReaderTesting
{
    [TestClass]
    public class StockComparerTests
    {
        #region constructor tests
        [TestMethod]
        public void TestDictionaryConstructor()
        {

            StockState ss = new StockState();

            Dictionary<string, Stock> allStocks = ss.Deserialize("stocksWithIndicators.bin");

            StockComparer sc = new StockComparer(allStocks);
            
        }
        #endregion

        #region method tests
        [TestMethod]
        public void TestRankOfBestStocks()
        {
            StockState ss = new StockState();

            Dictionary<string, Stock> allStocks = ss.Deserialize("stocksWithIndicators.bin");
            
            //StockComparer sc = new StockComparer(allStocks);
            List<Stock> rank = StockComparer.RankOfBestStocks(allStocks);
        }

        [TestMethod]
        public void TestRemoveStocksNotTradedEveryday()
        {
            StockState ss = new StockState();

            Dictionary<string, Stock> allStocks = ss.Deserialize("stocksWithIndicators.bin");

            List<Stock> dailyTradedStocks = StockComparer.RemoveStocksNotTradedEveryday(allStocks);

            //StockComparer sc = new StockComparer(allStocks);
            List <Stock> rank = StockComparer.RankOfBestStocks(dailyTradedStocks);

            StockState sc = new StockState(rank);
            sc.Serialize("rankedStocks.bin");

        }

        [TestMethod]
        public void TestRankStocksByCompare()
        {
            StockState ss = new StockState();

            Dictionary<string, Stock> allStocks = ss.Deserialize("stocksWithIndicators.bin");

            List<Stock> tradedEveryDay = StockComparer.RemoveStocksNotTradedEveryday(allStocks);

            StockComparer sc = new StockComparer(tradedEveryDay);

            sc.RankStocksByCompare();


        }
        #endregion
    }
}
