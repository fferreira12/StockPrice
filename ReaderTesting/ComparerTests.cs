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

        List<string> allLines;
        Dictionary<string, Stock> allStocks;
        Stock petr;

        [TestInitialize]
        public void Initialize()
        {
            allLines = Reader.GettAllLinesFromText(ReaderTesting.Properties.Resources.COTAHIST_A2016);
            allStocks = Reader.GetAllStockData(allLines);
            petr = allStocks["PETR3"];
        }

        [TestMethod]
        public void TestRankOfBestStocks() //static
        {

            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            MarketHistoryAnalyzer.FillAllWithDefaults(allStocks);

            List<Stock> stkRank = StockComparer.RankOfBestStocks(allStocks);

        }

        [TestMethod]
        public void TestRankObjectLevel()
        {

            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            MarketHistoryAnalyzer.FillAllWithDefaults(allStocks);

            StockComparer sc = new StockComparer(allStocks);

            sc.RankBestStocks();

            List<Stock> rank = sc.RankedStocks;

        }
    }
}
