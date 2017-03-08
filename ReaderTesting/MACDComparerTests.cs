using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;

namespace ReaderTesting
{
    [TestClass]
    public class MACDComparerTests
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
        public void TestGetDaysSinceLastReversal()
        {

            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");
            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            //Stock stk = allStocks["PETR3"];

            int daysSinceLastMACDReversal = MACDComparer.GetDaysSinceLastReversal(petr);
        }

        [TestMethod]
        public void TestGetMACDOpenness()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");
            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            //Stock stk = allStocks["PETR3"];

            decimal MACDOpenness = MACDComparer.GetMACDOpenness(petr);
        }

        [TestMethod]
        public void TestGetRankOfMACDOpenness()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");
            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            List<Stock> tradedEveryday = StockComparer.RemoveStocksNotTradedEveryday(allStocks);

            List<Stock> lst = MACDComparer.GetRankOfMACDOpenness(tradedEveryday);
        }
    }
}
