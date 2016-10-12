using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;

namespace ReaderTesting
{
    [TestClass]
    public class MarketHistoryAnalyzerTests
    {
        [TestMethod]
        public void TestFillSimpleMovingAvg()
        {

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillSimpleMovingAvg(ref petr);

        }

        [TestMethod]
        public void TestFillExponentialMovingAvg()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillExponentialMovingAvg(ref petr);
        }

        [TestMethod]
        public void TestFillAccDist()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillAccDist(ref petr, -306154896.4039m);
        }

        [TestMethod]
        public void TestFillRSI()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillRSI(ref petr, 14);
        }

        [TestMethod]
        public void TestFillAroon()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillAroon(ref petr, 14);
        }
    }
}
