using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;
using System.Linq;

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
            stk.MarketDatas.Add("20160104", m1);
            stk.MarketDatas.Add("20160105", m2);
            stk.MarketDatas.Add("20160106", m3);

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
            stk.MarketDatas.Add("20160104", m1);
            stk.MarketDatas.Add("20160105", m2);
            stk.MarketDatas.Add("20160106", m3);
            stk.MarketDatas.Add("20160107", m4);
            stk.MarketDatas.Add("20160108", m5);

            decimal EMA1 = Analyzer.ExponentialMovingAvg(stk, 3);
            decimal EMA2 = Analyzer.ExponentialMovingAvg(stk, 3, "20160107");

            Assert.AreEqual(33.75m, EMA1);
            Assert.AreEqual(17.5m, EMA2);

        }

        [TestMethod]
        public void TestEMA2()
        {

            List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            Stock petr = new Stock();
            petr.MarketDatas = mData;

            decimal EMA = Analyzer.ExponentialMovingAvg(petr, 9);
        }

        [TestMethod]
        public void TestAccDist()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            Stock petr = new Stock();
            petr.MarketDatas = mData;

            decimal AccDist = Analyzer.AccumulationDistribution(petr, startValue:-295529140.2642m);
        }

        [TestMethod]
        public void TestRSI()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);
            Dictionary<string, decimal> allRSI = new Dictionary<string, decimal>();
            Dictionary<string, decimal> allOverbought = new Dictionary<string, decimal>();
            Dictionary<string, decimal> allOversold = new Dictionary<string, decimal>();

            foreach(KeyValuePair<string, Stock> kvp in allStocks)
            {
                decimal RSI = Analyzer.RelativeStrenghtIndex(kvp.Value, 14);
                allRSI.Add(kvp.Key, RSI);
                if(RSI >= 70)
                {
                    allOverbought.Add(kvp.Key, RSI);
                }
                else if (RSI <= 30)
                {
                    allOversold.Add(kvp.Key, RSI);
                }
                Assert.IsTrue(RSI >= 0m);
                Assert.IsTrue(RSI <= 100m);
            }

            var orderedOverbought =
                from entry in allOverbought
                orderby entry.Value descending
                select entry;

            var orderedOversold =
                from entry in allOversold
                orderby entry.Value descending
                select entry;

        }

        [TestMethod]
        public void TestAroonOsc()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            decimal aOsc = Analyzer.AroonOscillator(stk, 14);

        }

        [TestMethod]
        public void TestMACD()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            decimal MACD = Analyzer.MACD(stk, 12, 26, 9, "20160819");
        }

        [TestMethod]
        public void TestROC()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            decimal ROC = Analyzer.RateOfChange(stk, 9);
        }

        [TestMethod]
        public void TestAnalyzeStock()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            Analyzer.AnalyzeStock(stk);
        }

        [TestMethod]
        public void TestFillSimpleMovingAvg()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            Analyzer.FillSimpleMovingAvg(ref stk);
        }

        [TestMethod]
        public void TestFillExponentialMovingAvg()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            Analyzer.FillExponentialMovingAvg(ref stk, 9);

            Assert.AreEqual(15.1065m, Math.Round(stk.indicators.EMAShort["20160926"],4));
        }

        [TestMethod]
        public void TestFillAccumulationDistribution()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            Analyzer.FillAccumulationDistribution(ref stk, -293444196.4039m);
        }

        [TestMethod]
        public void TestFillRSI()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            Analyzer.FillRelativeStrenghtIndex(stk, 14);

            Assert.AreEqual(47.0335m, Math.Round(stk.indicators.RSI["20160926"], 4));
        }

        [TestMethod]
        public void TestFillAroonOscillator()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            Stock stk = allStocks["PETR3"];

            Analyzer.FillAroonOscillator(stk, 14);

            Assert.AreEqual(14.2857m - 100m, Math.Round(stk.indicators.AroonOsc["20160926"], 4));
        }
    }
}
