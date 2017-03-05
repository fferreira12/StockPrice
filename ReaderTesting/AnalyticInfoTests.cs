using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;

namespace ReaderTesting
{
    [TestClass]
    public class AnalyticInfoTests
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
        public void TestQuantityOfIndicators()
        {

            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            //Stock petr = allStocks["PETR3"];

            int indsBefore = petr.indicators.QuantityOfIndicators;

            MarketHistoryAnalyzer.FillAllWithDefaults(petr);

            int indsAfter = petr.indicators.QuantityOfIndicators;

        }

        [TestMethod]
        public void TestRecalculate()
        {
            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            //Stock petr = allStocks["PETR3"];

            int indsBefore = petr.indicators.QuantityOfIndicators;

            petr.indicators.Recalculate();

            int indsAfter = petr.indicators.QuantityOfIndicators;
        }

        [TestMethod]
        public void TestPunctuate()
        {
            //Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            //Stock petr = allStocks["VALE3"];

            decimal points = petr.indicators.Punctuation;

            Assert.AreNotEqual(0m, points);
        }
    }
}
