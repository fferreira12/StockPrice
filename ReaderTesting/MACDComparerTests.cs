using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;

namespace ReaderTesting
{
    [TestClass]
    public class MACDComparerTests
    {
        [TestMethod]
        public void TestGetDaysSinceLastReversal()
        {

            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            Stock stk = allStocks["PETR3"];

            int daysSinceLastMACDReversal = MACDComparer.GetDaysSinceLastReversal(stk);
        }

        [TestMethod]
        public void TestGetMACDOpenness()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            Stock stk = allStocks["PETR3"];

            decimal MACDOpenness = MACDComparer.GetMACDOpenness(stk);
        }

        [TestMethod]
        public void TestGetRankOfMACDOpenness()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            List<Stock> tradedEveryday = StockComparer.RemoveStocksNotTradedEveryday(allStocks);

            List<Stock> lst = MACDComparer.GetRankOfMACDOpenness(tradedEveryday);
        }
    }
}
