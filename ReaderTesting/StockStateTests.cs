using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;
using System.IO;

namespace ReaderTesting
{
    [TestClass]
    public class StockStateTests
    {
        [TestMethod]
        public void TestSerialize()
        {

            //CONTINUE HERE
            string s = Properties.Resources.COTAHIST_A2016;
            List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);

            StockState sc = new StockState(allStocks);

            sc.Serialize("stocks.bin");

            Assert.IsTrue(File.Exists("stocks.bin"));
        }

        [TestMethod]
        public void TestDeserialize()
        {
            StockState sc = new StockState();

            Dictionary<string, Stock> allStocks = sc.Deserialize("stocks.bin");

            Assert.AreNotEqual(null, allStocks);
        }
    }
}
