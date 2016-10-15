using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;

namespace ReaderTesting
{
    [TestClass]
    public class AnalyticInfoTests
    {
        [TestMethod]
        public void TestQuantityOfIndicators()
        {

            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            int indsBefore = petr.indicators.QuantityOfIndicators;

            MarketHistoryAnalyzer.FillAllWithDefaults(petr);

            int indsAfter = petr.indicators.QuantityOfIndicators;

        }

        [TestMethod]
        public void TestRecalculate()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Stock petr = allStocks["PETR3"];

            int indsBefore = petr.indicators.QuantityOfIndicators;

            petr.indicators.Recalculate();

            int indsAfter = petr.indicators.QuantityOfIndicators;
        }
    }
}
