using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;

namespace ReaderTesting
{
    [TestClass]
    public class AnalyzerTests
    {
        [TestMethod]
        public void TestSimpleMovingAvg()
        {
            MarketData m1 = new MarketData();
            m1.closePrice = 10m;
            MarketData m2 = new MarketData();
            m2.closePrice = 15m;
            MarketData m3 = new MarketData();
            m3.closePrice = 20m;

            Stock stk = new Stock();
            stk.marketHistory.Add("20160104", m1);
            stk.marketHistory.Add("20160105", m2);
            stk.marketHistory.Add("20160106", m3);

            decimal avg3 = Analyzer.SimpleMovingAvg(stk, 3);
            decimal avg2Limited = Analyzer.SimpleMovingAvg(stk, 2, "20160105");

            Assert.AreEqual(15m, avg3);
            Assert.AreEqual(12.5m, avg2Limited);
        }

        [TestMethod]
        public void TestDateTimeToString()
        {
            DateTime t = new DateTime(2016, 01, 01);

            string tStr = t.ToStringF();

            Assert.AreEqual("20160101", tStr);
        }

        [TestMethod]
        public void TestExponentialMovingAvg()
        {
            MarketData m1 = new MarketData();
            m1.closePrice = 10m;
            MarketData m2 = new MarketData();
            m2.closePrice = 10m;
            MarketData m3 = new MarketData();
            m3.closePrice = 10m;
            MarketData m4 = new MarketData();
            m4.closePrice = 25m;
            MarketData m5 = new MarketData();
            m5.closePrice = 50m;

            Stock stk = new Stock();
            stk.marketHistory.Add("20160104", m1);
            stk.marketHistory.Add("20160105", m2);
            stk.marketHistory.Add("20160106", m3);
            stk.marketHistory.Add("20160107", m4);
            stk.marketHistory.Add("20160108", m5);

            decimal EMA1 = Analyzer.ExponentialMovingAvg(stk, 3);
            decimal EMA2 = Analyzer.ExponentialMovingAvg(stk, 3, "20160107");

            Assert.AreEqual(33.75m, EMA1);
            Assert.AreEqual(17.5m, EMA2);

        }

        [TestMethod]
        public void TestEMA2()
        {

            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            Stock petr = new Stock();
            petr.marketHistory = mData;

            decimal EMA = Analyzer.ExponentialMovingAvg(petr, 9);
        }

        [TestMethod]
        public void TestAccDist()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            Stock petr = new Stock();
            petr.marketHistory = mData;

            decimal AccDist = Analyzer.AccumulationDistribution(petr);
        }

        [TestMethod]
        public void TestRSI()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            foreach(KeyValuePair<string, Stock> kvp in allStocks)
            {
                decimal RSI = Analyzer.RelativeStrenghtIndex(kvp.Value, 14);
                Assert.IsTrue(RSI >= 0m);
                Assert.IsTrue(RSI <= 100m);
            }
        }
    }
}
