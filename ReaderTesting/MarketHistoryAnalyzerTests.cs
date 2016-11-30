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

        [TestMethod]
        public void TestFillMACD()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillMACD(ref petr, 12, 26, 9);
        }

        [TestMethod]
        public void TestFillROC()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillROC(ref petr, 9);
        }

        [TestMethod]
        public void TestFillAllWithDefaults()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            MarketHistoryAnalyzer.FillAllWithDefaults(petr);
        }

        [TestMethod]
        public void TestFillAllWithDefaultsWithList()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock stk1 = allStocks["PETR3"];
            Stock stk2 = allStocks["ABEV3"];
            Stock stk3 = allStocks["BBAS3"];
            Stock stk4 = allStocks["BBDC3"];
            Stock stk5 = allStocks["EMBR3"];

            List<Stock> allS = new List<Stock>();
            allS.Add(stk1);
            allS.Add(stk2);
            allS.Add(stk3);
            allS.Add(stk4);
            allS.Add(stk5);

            MarketHistoryAnalyzer.FillAllWithDefaults(allS);
        }

        [TestMethod]
        public void TestFillAllWithDefaultsWithDictionary()
        {

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            MarketHistoryAnalyzer.FillAllWithDefaults(allStocks);

        }
    }
}
