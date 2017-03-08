using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;

namespace ReaderTesting
{
    [TestClass]
    public class MarketHistoryTests
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

        #region constructor tests
        [TestMethod]
        public void TestCreateMarketHistoryWithVoidConstructor()
        {

            MarketHistory mh = new MarketHistory(new Dictionary<string, MarketData>());

        }

        [TestMethod]
        public void TestCreateMarketHistoryWithDictionary()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

        }
        #endregion

        #region indexer tests
        [TestMethod]
        public void TestStringIndexer()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            MarketData retrieved = mh["20160926"];
        }

        [TestMethod]
        public void TestIntIndexer()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            MarketData retrieved = mh[429];
        }
        #endregion

        #region properties tests
        [TestMethod]
        public void TestDatesProperty()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            List<string> dates = mh.Dates;
        }
        #endregion

        #region methods tests
        [TestMethod]
        public void TestGetLastNMarKetDatas()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            var last15 = mh.GetLastNMarKetDatas(15, mh[225].dateStr);
        }

        [TestMethod]
        public void TestGetLastNClosingPrices()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            var last15 = mh.GetLastNClosingPrices(15, mh[225].dateStr);
        }

        [TestMethod]
        public void TestGetLastNumericData()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            var avg = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.AVGPRICE);
            var close = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.CLOSEPRICE);
            var mkt = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.MARKETTYPE);
            var max = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.MAXPRICE);
            var min = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.MINPRICE);
            var neg = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.NEGOTIATIONSNUMBER);
            var open = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.OPENPRICE);
            var papers = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.PAPERSNUMBER);
            var vol = mh.GetLastNumericData(15, mh[225].dateStr, MarketNumericInfo.VOLUME);

           
        }
        #endregion

        #region enumerator tests

        [TestMethod]
        public void TestForEach()
        {
            //List<string> allLines = Reader.GetAllLinesFromPath("COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            int mDatas = 0;

            foreach(MarketData m in mh)
            {
                mDatas++;
            }
        }

        #endregion
    }
}
