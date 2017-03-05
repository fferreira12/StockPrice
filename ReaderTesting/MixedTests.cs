using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;
using System.Linq;

namespace ReaderTesting
{
    [TestClass]
    public class MixedTests
    {
        [TestMethod]
        public void GetStockWithHigherRSI()
        {
            
            //get all stocks
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            decimal maxVal = 0m;
            Stock stk = null;

            foreach(KeyValuePair<string, Stock> kvp in allStocks)
            {
                decimal RSI = Analyzer.RelativeStrenghtIndex(kvp.Value, 14);

                if (RSI > maxVal)
                {
                    maxVal = RSI;
                    stk = kvp.Value;
                }
            }

        }

        [TestMethod]
        public void GetHistoryOfAroon()
        {
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");
            Stock stk = allStocks["ABEV3"];

            List<string> mDates =
                (from string d in stk.MarketDatas.Keys
                 orderby d ascending
                 select d).ToList();

            Dictionary<string, decimal> aroonHist = new Dictionary<string, decimal>();

            foreach (string date in mDates)
            {
                aroonHist.Add(date, Analyzer.AroonOscillator(stk, 14, date));
            }

        }

        [TestMethod]
        public void TestGetStockWithHigherROC()
        {
            //get all stocks
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("COTAHIST_A2016.TXT");

            decimal maxVal = 0m;
            Stock stk = null;

            foreach (KeyValuePair<string, Stock> kvp in allStocks)
            {
                decimal ROC = Analyzer.RateOfChange(kvp.Value, 14);

                if (ROC > maxVal)
                {
                    maxVal = ROC;
                    stk = kvp.Value;
                }
            }
        }
    }
}
