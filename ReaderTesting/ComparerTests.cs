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
        public void TestRankOfBestStocks() //static
        {

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            MarketHistoryAnalyzer.FillAllWithDefaults(allStocks);

            List<Stock> stkRank = StockComparer.RankOfBestStocks(allStocks);

        }

        [TestMethod]
        public void TestRankObjectLevel()
        {

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            MarketHistoryAnalyzer.FillAllWithDefaults(allStocks);

            StockComparer sc = new StockComparer(allStocks);

            sc.RankBestStocks();

            List<Stock> rank = sc.RankedStocks;

        }
    }
}
