using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPrice;

namespace StockPrice.Tests
{
    [TestClass()]
    public class ReaderTests
    {
        [TestMethod()]
        public void GettAllLinesFromTextTest()
        {
            string t = ReaderTesting.Properties.Resources.COTAHIST_A2016;
            List<string> l = Reader.GettAllLinesFromText(t);

        }
    }
}