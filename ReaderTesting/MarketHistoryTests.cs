using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;

namespace ReaderTesting
{
    [TestClass]
    public class MarketHistoryTests
    {
        #region constructor tests
        [TestMethod]
        public void TestCreateMarketHistoryWithVoidConstructor()
        {

            MarketHistory mh = new MarketHistory();

        }

        [TestMethod]
        public void TestCreateMarketHistoryWithDictionary()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

        }
        #endregion

        #region indexer tests
        [TestMethod]
        public void TestStringIndexer()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            MarketData retrieved = mh["20160926"];
        }

        [TestMethod]
        public void TestIntIndexer()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            MarketData retrieved = mh[429];
        }
        #endregion

        #region properties tests
        [TestMethod]
        public void TestDatesProperty()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            List<string> dates = mh.Dates;
        }
        #endregion

        #region methods tests
        [TestMethod]
        public void TestGetLastNMarKetDatas()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            var last15 = mh.GetLastNMarKetDatas(15, mh[429].dateStr);
        }

        [TestMethod]
        public void TestGetLastNClosingPrices()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

            Dictionary<string, MarketData> mData = Reader.GetMarketDataFromPaper("PETR3", allLines);

            MarketHistory mh = new MarketHistory(mData);

            var last15 = mh.GetLastNClosingPrices(15, mh[429].dateStr);
        }
        #endregion

        #region enumerator tests

        [TestMethod]
        public void TestForEach()
        {
            List<string> allLines = Reader.GetAllLinesFromPath("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

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
