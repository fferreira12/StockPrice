using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;

namespace ReaderTesting
{
    [TestClass]
    public class StockStateTests
    {
        [TestMethod]
        public void TestSerialize()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            StockState sc = new StockState(allStocks);

            sc.Serialize("stocks.bin");
        }

        [TestMethod]
        public void TestDeserialize()
        {
            StockState sc = new StockState();

            Dictionary<string, Stock> allStocks = sc.Deserialize("stocks.bin");
        }
    }
}
