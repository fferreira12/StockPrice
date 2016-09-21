using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockPrice;

namespace ReaderTesting
{
    [TestClass]
    public class MixedTests
    {
        [TestMethod]
        public void GetStockWithHigherRSI()
        {
            
            //get all stocks
            Dictionary<string, Stock> allStocks = Reader.GetAllStockData("C:\\Users\\Cliente\\Downloads\\COTAHIST_A2016.TXT");

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
    }
}
