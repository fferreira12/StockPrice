using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.IO;
using System.Collections.Generic;

namespace ReaderTesting
{
    [TestClass]
    public class ReaderTests
    {
        [TestMethod]
        public void TestGetReadableStream()
        {

            StreamReader sr = Reader.GetReadableStream("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

        }

        [TestMethod]
        public void TestReadLines()
        {

            StreamReader sr = Reader.GetReadableStream("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            List<string> strings = null;

            strings = Reader.ReadLines(sr, 0);

        }

        [TestMethod]
        public void TestReadSubstring()
        {
            string str = "012016091682VALEW14     080VALEE       PNA     N1000R$  000000000010600000000001100000000000104000000000010500000000001100000000000000000000000000000003000000000000002400000000000000254000000000000140002016112100000010000000000000BRVALEACNPA3192";

            string sub = Reader.ReadSubstring(str, 1, 3);

            Assert.AreEqual("012", sub);


        }

        [TestMethod]
        public void TestGetStringInfo()
        {
            string str = "012016091682VALEW14     080VALEE       PNA     N1000R$  000000000010600000000001100000000000104000000000010500000000001100000000000000000000000000000003000000000000002400000000000000254000000000000140002016112100000010000000000000BRVALEACNPA3192";

            string name = Reader.GetStringInfo(str, MarketStringInfo.BIZNAME);
            string curr = Reader.GetStringInfo(str, MarketStringInfo.CURRENCY);
            string date = Reader.GetStringInfo(str, MarketStringInfo.DATE);
            string code = Reader.GetStringInfo(str, MarketStringInfo.PAPERCODE);

            Assert.AreEqual("VALEE", name);
            Assert.AreEqual("R$", curr);
            Assert.AreEqual("20160916", date);
            Assert.AreEqual("VALEW14", code);
        }

        [TestMethod]
        public void TestGetNumericInfo()
        {
            string str = "012016022902VVAR11      010VIAVAREJO   UNT     N2   R$  000000000041700000000004450000000000414000000000042800000000004330000000000433000000000044003063000000000000833800000000000357418300000000000000009999123100000010000000000000BRVVARCDAM10101";

            decimal min = Reader.GetNumericInfo(str, MarketNumericInfo.MINPRICE);
            decimal max = Reader.GetNumericInfo(str, MarketNumericInfo.MAXPRICE);
            decimal open = Reader.GetNumericInfo(str, MarketNumericInfo.OPENPRICE);
            decimal close = Reader.GetNumericInfo(str, MarketNumericInfo.CLOSEPRICE);
            decimal avg = Reader.GetNumericInfo(str, MarketNumericInfo.AVGPRICE);

            decimal neg = Reader.GetNumericInfo(str, MarketNumericInfo.NEGOTIATIONSNUMBER);
            decimal pap = Reader.GetNumericInfo(str, MarketNumericInfo.PAPERSNUMBER);
            decimal vol = Reader.GetNumericInfo(str, MarketNumericInfo.VOLUME);

        }

        [TestMethod]
        public void TestConvertStringToDateTime()
        {
            DateTime d = Reader.ConvertStringToDateTime("20160918");
        }

        [TestMethod]
        public void TestGetMarketDataFromPaper()
        {

            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("UCAS3", allLines);

        }

        [TestMethod]
        public void TestGetAllStockData()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData(allLines);
        }

        

    }
}
