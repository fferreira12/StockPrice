using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;
using System.Collections;

namespace ReaderTesting
{
    [TestClass]
    public class ComparerTests
    {
        [TestMethod]
        public void TestRankOfBestStocks()
        {

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            MarketHistoryAnalyzer.FillAllWithDefaults(allStocks);

            List<Stock> stkRank = StockComparer.RankOfBestStocks(allStocks);

        }
    }
}
