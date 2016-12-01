using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System.Collections.Generic;
using System.Text;

namespace ReaderTesting
{
    [TestClass]
    public class EmailNotifierTests
    {
        [TestMethod]
        public void TestEmailNotifier()
        {
            //the contents of the email
            StockState ss = new StockState();
            Dictionary<string, Stock> rankDic = ss.Deserialize("rankedStocks.bin");
            
            EmailNotifier en = new EmailNotifier("ff12sender@gmail.com", "33914047");

            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (KeyValuePair<string, Stock> s in rankDic)
            {
                sb.Append(i + ". " + s.Value.stockCode + "\n");
                i++;
            }

            en.Send("StockMarket Rank", sb.ToString());

        }
    }
}
